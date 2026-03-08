using Cysharp.Threading.Tasks;
using Marmalade.Core;
using VContainer.Unity;
using Marmalade.Shared;

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
        private readonly ISettingsService _settingsService;
        private readonly ISaveService _saveService;

        public BootstrapEntry(
            ISceneService sceneService,
            IQualityService qualityService,
            ISettingsService settingsService,
            ISaveService saveService)
        {
            _sceneService    = sceneService;
            _qualityService  = qualityService;
            _settingsService = settingsService;
            _saveService     = saveService;
        }

        /// <summary>
        /// Called automatically by VContainer after all dependencies have been injected.
        /// Executes all global startup initialisation in order.
        /// </summary>
        public void Initialize()
        {
            RegisterExceptionHandler();
            _qualityService.InitialiseQuality();

            // Settings must be loaded before save data — audio, display, and
            // other systems may depend on settings values during their own init.
            _settingsService.Load();
            _saveService.Load();

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