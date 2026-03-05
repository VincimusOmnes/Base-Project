using System;
using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using UnityEngine.SceneManagement;
using Marmalade.Core;
using Marmalade.Shared;

namespace Marmalade.Systems
{
    /// <summary>
    /// Manages all scene loading and unloading for the application.
    /// All scene transitions should go through this service rather than calling SceneManager directly,
    /// to ensure consistent loading screen behaviour, progress reporting, and cancellation token support.
    /// Publishes SceneLoadStartedMessage, SceneLoadProgressMessage, and SceneLoadCompletedMessage
    /// via MessagePipe.
    /// </summary>
    public class SceneService : ISceneService
    {
        private readonly IPublisher<SceneLoadStartedMessage> _loadStarted;
        private readonly IPublisher<SceneLoadProgressMessage> _loadProgress;
        private readonly IPublisher<SceneLoadCompletedMessage> _loadCompleted;
        private readonly LoadingScreenConfig _config;
        private readonly HashSet<string> _loadedScenes = new();

        /// <summary>
        /// Initialises the service with the publishers required to report scene load progress
        /// and the config asset that controls loading screen behaviour.
        /// </summary>
        public SceneService(
            LoadingScreenConfig config,
            IPublisher<SceneLoadStartedMessage> loadStarted,
            IPublisher<SceneLoadProgressMessage> loadProgress,
            IPublisher<SceneLoadCompletedMessage> loadCompleted)
        {
            _config = config;
            _loadStarted = loadStarted;
            _loadProgress = loadProgress;
            _loadCompleted = loadCompleted;
        }

        /// <summary>
        /// Additively loads the scene with the given name.
        /// Publishes SceneLoadStartedMessage and SceneLoadCompletedMessage at the start and end.
        /// Waits for both the scene to finish loading and the minimum display time before completing.
        /// </summary>
        public async UniTask LoadSceneAsync(string sceneName, CancellationToken ct = default)
        {
            _loadStarted.Publish(new SceneLoadStartedMessage(sceneName));

            UniTask loadTask = LoadSceneInternalAsync(sceneName, ct);
            UniTask minimumTask = UniTask.Delay(
                TimeSpan.FromSeconds(_config.MinimumDisplaySeconds),
                cancellationToken: ct);

            await UniTask.WhenAll(loadTask, minimumTask);

            _loadCompleted.Publish(new SceneLoadCompletedMessage(sceneName));
        }

        /// <summary>
        /// Unloads the scene with the given name.
        /// Logs a warning and returns if the scene is not currently loaded.
        /// </summary>
        public async UniTask UnloadSceneAsync(string sceneName, CancellationToken ct = default)
        {
            if (!_loadedScenes.Contains(sceneName))
            {
                Log.Warning($"Attempted to unload scene '{sceneName}' that is not loaded.");
                return;
            }

            await SceneManager.UnloadSceneAsync(sceneName).ToUniTask(cancellationToken: ct);
            _loadedScenes.Remove(sceneName);
        }
        /// <summary>
        /// Additively loads a scene with proper CancellationToken handling
        /// Skips all load handling and is not added to list of loaded scenes.
        /// </summary>
        public async UniTask LoadPersistentSceneAsync(string sceneName, CancellationToken ct = default)
        {
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask(cancellationToken: ct);
        }

        private async UniTask LoadSceneInternalAsync(string sceneName, CancellationToken ct)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // Unity returns null if the scene is not in the build profile.
            // Throwing here surfaces a clear error rather than a NullReferenceException.
            if (operation == null)
            {
                throw new InvalidOperationException(
                    $"Scene '{sceneName}' could not be loaded. Make sure it is added to the build profile.");
            }

            operation.allowSceneActivation = false;

            // Unity's AsyncOperation only goes up to 0.9 when allowSceneActivation is false.
            // Dividing by 0.9 normalises it to a 0-1 range for the progress message.
            while (operation.progress < 0.9f)
            {
                _loadProgress.Publish(new SceneLoadProgressMessage(operation.progress / 0.9f));
                await UniTask.Yield(ct);
            }

            _loadProgress.Publish(new SceneLoadProgressMessage(1f));
            operation.allowSceneActivation = true;
            await operation.ToUniTask(cancellationToken: ct);

            _loadedScenes.Add(sceneName);
        }
    }
}