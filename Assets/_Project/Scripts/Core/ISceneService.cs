using System.Threading;
using Cysharp.Threading.Tasks;

namespace Marmalade.Core
{
    /// <summary>
    /// Contract for the scene management service.
    /// All scene loading and unloading should go through this service rather than
    /// calling SceneManager directly, to ensure consistent loading screen behaviour,
    /// progress reporting, and cancellation token support throughout the project.
    /// </summary>
    public interface ISceneService
    {
        /// <summary>
        /// Additively loads the scene with the given name.
        /// Implementations should report progress via SceneLoadProgressMessage and publish
        /// SceneLoadStartedMessage and SceneLoadCompletedMessage at the start and end.
        /// </summary>
        UniTask LoadSceneAsync(string sceneName, CancellationToken ct = default);

        /// <summary>
        /// Unloads the scene with the given name.
        /// Implementations should handle the case where the scene is not currently loaded gracefully.
        /// </summary>
        UniTask UnloadSceneAsync(string sceneName, CancellationToken ct = default);

        /// <summary>
        /// Loads the scene by name skipping any internal load handling.
        /// Does not add to DoNotDestroy
        /// </summary>
        UniTask LoadPersistentSceneAsync(string sceneName, CancellationToken ct = default);
    }
}