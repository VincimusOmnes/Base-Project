using System;
using Cysharp.Threading.Tasks;
using Marmalade.Core;
using Marmalade.Shared;
using VContainer.Unity;
using UnityEngine.SceneManagement;

namespace Marmalade.Bootstrap
{
    /// <summary>
    /// Entry point for the Bootstrap scene, executed once at application startup by VContainer.
    /// Responsible for initialising global systems that must be configured before any scene loads.
    /// Register additional startup logic here by calling private initialisation methods from Initialize().
    /// </summary>
    public class BootstrapEntry : IInitializable
    {
        private readonly ISceneService _sceneService;
        private readonly IQualityService _qualityService;

        public BootstrapEntry(ISceneService sceneService, IQualityService qualityService)
        {
            _sceneService = sceneService;
            _qualityService = qualityService;
        }

        /// <summary>
        /// Called automatically by VContainer after all dependencies have been injected.
        /// Executes all global startup initialisation in order.
        /// </summary>
        public void Initialize()
        {
            RegisterExceptionHandler();
            _qualityService.InitialiseQuality();
            StartupAsync().Forget();
        }

        private static void RegisterExceptionHandler()
        {
            UniTaskScheduler.UnobservedTaskException += ex =>
            {
                #if ENABLE_LOGGING
                    Log.Exception(ex);
                #endif
            };
        }

        private async UniTaskVoid StartupAsync()
        {
            // LoadingScreen is loaded first and never unloaded — it persists for the
            // lifetime of the application and overlays all subsequent scene transitions.
            await _sceneService.LoadPersistentSceneAsync("LoadingScreen");
            await _sceneService.LoadSceneAsync("MainMenu");
        }
    }
}