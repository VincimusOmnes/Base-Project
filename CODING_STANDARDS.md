# Marmalade — Coding Standards

## Architecture

### Assembly Dependency Direction
```
Shared ← Core ← Systems ← Bootstrap
                ↑
            Features (reference Core only, never Systems)
```

| Assembly | Purpose |
|---|---|
| `Marmalade.Shared` | Utilities, messages, ThemeConfig. No dependencies. |
| `Marmalade.Core` | Interfaces for shared systems. References Shared only. |
| `Marmalade.Systems` | Implementations of Core interfaces. |
| `Marmalade.Bootstrap` | Composition root. Wires everything together. |
| `Marmalade.Feature.*` | Self-contained features. Reference Core and Shared only. |
| `Marmalade.Editor` | Editor-only tooling. Stripped from builds. |
| `Marmalade.Tests` | Test assemblies. |

- Interfaces for shared systems → `Marmalade.Core`
- Implementations → `Marmalade.Systems`
- Feature-specific interfaces → live in their own feature assembly unless multiple assemblies need them
- All asmdefs: **Auto Referenced off**, Use GUIDs on, Root Namespace = assembly name

**Why Auto Referenced off?** When Auto Referenced is on, Unity automatically makes your assembly visible to everything else. Turning it off means an assembly can only see your code if it explicitly declares the reference in its asmdef. This enforces the dependency directions above — nothing can accidentally depend on an assembly without making that dependency visible and intentional. The exception is `Marmalade.Editor`, which has Auto Referenced on so Unity can automatically discover editor attributes like `[InitializeOnLoad]` and `[MenuItem]`.

### VContainer
- `BootstrapLifetimeScope` is the root — all global singletons registered here
- Child `LifetimeScope` per scene for presenters and views
- Constructor: store injected dependencies only — no work here
- `Initialize()`: use dependencies, call methods, start async work

**Why no work in the constructor?** When VContainer calls your constructor, it is in the middle of building the container. Not all dependencies are fully set up yet. `Initialize()` is called after the container is fully built — it is VContainer's explicit signal that everything is wired up and ready. Think of it as the equivalent of `Start()` on a MonoBehaviour:

| MonoBehaviour | VContainer plain C# |
|---|---|
| `Awake()` | Constructor |
| `Start()` | `Initialize()` |
| `Update()` | `ITickable.Tick()` |
| `OnDestroy()` | `IDisposable.Dispose()` |

### MonoBehaviour
Only use MonoBehaviour for: scene-attached objects, physics callbacks, editor callbacks, View classes. Everything else is plain C#.

---

## Naming

### Namespaces
Namespace always matches the asmdef name exactly. Folder structure organises files but does not affect namespace.

### C#
| | Convention | Example |
|---|---|---|
| Classes | PascalCase | `SceneService` |
| Interfaces | `I` prefix | `ISceneService` |
| Private fields | `_camelCase` | `_sceneService` |
| Public properties | PascalCase | `MinimumDisplaySeconds` |
| Methods | PascalCase | `LoadSceneAsync` |
| Local variables | camelCase | `sceneName` |

### Member Order
```
[SerializeField] private fields     ← Inspector-facing
Private fields                      ← internal state
Public properties                   ← programmatic access
Unity Messages                      ← Awake, Start, OnEnable, OnDisable, OnDestroy
Public Methods
Private Methods
```

### Fields and Properties
- Never use public fields
- `[SerializeField] private` for Inspector exposure on MonoBehaviours
- `[field: SerializeField] public ... { get; private set; }` for ScriptableObject properties
- Public properties for programmatic read access where needed

### Unity Hierarchy
Full context + type suffix. Name must identify what the object is without expanding the hierarchy.

- PascalCase, no spaces
- Suffix with the Unity component type that matters: `Image`, `Canvas`, `Text`, `Button`
- Pure layout containers (RectTransform only) don't need a type suffix
- Sub-components use the parent as context prefix

```
✓ LoadingScreenCanvas
✓ ProgressBarFillImage
✓ LogoImage
✓ ProgressBar          ← pure layout container, no suffix needed
✗ Image (1)            ← no context
✗ Background           ← ambiguous
```

### Script Placement
Each script gets its own dedicated empty GameObject named after the script. Do not put scripts on GameObjects that also own visual components, and do not stack multiple scripts on the scene root.

```
LoadingScreen (Scene)
├── LoadingScreenView       ← empty GO, holds LoadingScreenView.cs
├── LoadingScreenCanvas
│   ├── BackgroundImage
│   ├── LogoImage
│   ├── ProgressBar
│   │   └── ProgressBarFillImage
│   └── StatusText
└── EventSystem
```

This scales cleanly to scenes with many scripts — each one is immediately findable by name in the hierarchy.

### Assets
| | Convention |
|---|---|
| ScriptableObjects | PascalCase — `LoadingScreenConfig` |
| Prefabs | PascalCase — `LoadingScreen` |
| Scenes | PascalCase — `Bootstrap`, `MainMenu` |
| Textures | PascalCase + `_Tex` — `CompanyLogo_Tex` |
| Audio | PascalCase + `_SFX` or `_Music` — `ButtonClick_SFX` |

---

## Code Style

- **Always use explicit types** — never `var`
- **Always declare the interface type** when assigning an implementation: `ISceneService service = new SceneService()`

### Async / UniTask
- Every `async UniTask` method accepts `CancellationToken ct = default` — every async operation must be cancellable so it doesn't keep running after the object that started it is destroyed. `default` makes the token optional so callers without one don't need to pass it.
- MonoBehaviours: `this.GetCancellationTokenOnDestroy()` — automatically cancels when the MonoBehaviour is destroyed
- Plain C# services: `CancellationTokenSource`, cancel in `Dispose()` — same effect, manual mechanism since there is no Unity lifecycle
- Never drop the token mid-chain — if you call an async method without forwarding the token, that branch becomes untracked and keeps running even after everything else has stopped
- `UniTaskVoid` + `Forget()` only at top-level fire-and-forget entry points — everything below should return `UniTask` so tokens and exceptions flow correctly
- `Forget()` routes exceptions through `UniTaskScheduler.UnobservedTaskException` — without it, exceptions from fire-and-forget tasks are silently swallowed

### Logging
- Always use `Log.Info`, `Log.Warning`, `Log.Error`, `Log.Exception` — never `Debug.Log`
- `Log.Info` and `Log.Warning` compile out in release (`ENABLE_LOGGING`)
- `Log.Error` and `Log.Exception` are always present

### Comments
- Every public class: summary explaining its role in the architecture
- Every public method: summary explaining behaviour and non-obvious side effects
- Interface comments: use *"Implementations should..."*
- Private methods: only comment where reasoning is non-obvious
- Explain WHY and WHAT ROLE — the code shows WHAT

### Null Conditional Operator (?.)
- Use `?.` when null is a genuinely valid state and skipping the call is the correct behaviour
- Do not use `?.` when null would indicate a bug — let it throw so the problem is found
- Good uses: `_subscriptions?.Dispose()` in `Dispose()` since `Initialize()` may never have been called, and `OnSomethingHappened?.Invoke()` since event handlers are legitimately optional
- Bad use: `?.` on something that should never be null — it masks bugs and makes debugging harder

### ScriptableObject Config Pattern
- Assigned via `[SerializeField]` on the relevant `LifetimeScope`
- Registered with `builder.RegisterInstance()` — not `builder.Register<T>()`, because VContainer should not construct the asset, it already exists
- Injected into services via constructor, same as any other dependency
- Live in `_Project/Settings/` — this is the designer/programmer boundary. Designers own the data in Settings, programmers own the code. Keeping all config assets in one place means designers can tune values without touching code, and programmers know that changing a value in Settings won't break anything structural.
- Use `[CreateAssetMenu(menuName = "Marmalade Objects/...")]`

**`Register<T>()` vs `RegisterInstance()`** — use `Register<T>()` when you want VContainer to construct the object and inject its dependencies. Use `RegisterInstance()` when you already have the object (e.g. a ScriptableObject assigned in the Inspector) and just want VContainer to hand it out when something asks for it.

---

## MessagePipe

- Messages are `readonly struct` — zero allocation
- Messages live in `Marmalade.Systems` next to the service that publishes them
- Message names end in `Message` — `SceneLoadStartedMessage`
- Subscribe in `Initialize()`, never in the constructor — MessagePipe infrastructure may not be fully set up when the constructor runs. By `Initialize()`, the container is fully built and all dependencies are ready.
- `Subscribe()` returns `IDisposable` — always store and dispose it
- Use `DisposableBag` for multiple subscriptions

---

## UI

- uGUI (Canvas, Image, TMP) for all runtime UI — UI Toolkit is not used
- PrimeTween for all animations
- `CanvasGroup` alpha tween for fade in/out — never `SetActive` for animated elements

---

## Build Configuration

### Scripting Define Symbols
| Symbol | Purpose |
|---|---|
| `UNITASK_WEBGL_THREADING_SUPPORT` | Always present |
| `ENABLE_LOGGING` | Gates `Log.Info` and `Log.Warning` |
| `ENABLE_DEV_TOOLS` | Gates dev console |
| `ENABLE_CHEATS` | Gates cheat commands |

| Build | Active Symbols |
|---|---|
| Editor / Dev | `ENABLE_DEV_TOOLS` `ENABLE_CHEATS` `ENABLE_LOGGING` |
| QA | `ENABLE_CHEATS` `ENABLE_LOGGING` |
| Staging | `ENABLE_LOGGING` |
| Release | *(none)* |

### Scene Management
- Bootstrap is always scene 0 in the build profile
- All scenes loaded additively at runtime via `ISceneService` — never call `SceneManager` directly
- `BootstrapLoader` (Editor only) ensures Bootstrap loads first regardless of which scene is open in the Editor. In production, Bootstrap loads naturally as scene 0 and `BootstrapLoader` is stripped from the build.

---

## WebGL

- Never use `Task`, `Thread`, `Task.Run()`, or `new Thread()` — WebGL is single-threaded
- Never use `System.IO` — use `PlayerPrefs` as the save storage backend
- Audio requires a user interaction before it will play — implement audio context resume on first click
- Memory is a fixed heap — always release Addressables assets, leaks will crash the tab
- Test in a browser regularly — do not leave WebGL testing until the end

---

*Update this document whenever a new convention is established. If you deviate from a convention, document why with a code comment at the point of deviation.*
