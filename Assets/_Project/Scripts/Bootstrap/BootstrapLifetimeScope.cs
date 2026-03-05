using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using Marmalade.Core;
using Marmalade.Systems;

namespace Marmalade.Bootstrap
{
    /// <summary>
    /// Root VContainer LifetimeScope for the Bootstrap scene.
    /// Responsible for initialising global systems and registering application-wide services and message brokers.
    /// This scope is the parent of all scene-level child scopes and is never destroyed during the application lifetime.
    /// Register only truly global concerns here scene and system specific registrations belong in their own child scopes.
    /// </summary>
    public class BootstrapLifetimeScope : LifetimeScope
    {
        [SerializeField] private LoadingScreenConfig _loadingScreenConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            // Entry points
            builder.RegisterEntryPoint<BootstrapEntry>();

            // MessagePipe global setup
            MessagePipeOptions options = builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            // Config assets
            builder.RegisterInstance(_loadingScreenConfig);

            // Global message brokers
            builder.RegisterMessageBroker<GameStateChangedMessage>(options);

            // Scene management message brokers
            builder.RegisterMessageBroker<SceneLoadStartedMessage>(options);
            builder.RegisterMessageBroker<SceneLoadProgressMessage>(options);
            builder.RegisterMessageBroker<SceneLoadCompletedMessage>(options);

            // Services
            builder.Register<IGameStateService, GameStateService>(Lifetime.Singleton);
            builder.Register<IQualityService, QualityService>(Lifetime.Singleton);
            builder.Register<ISceneService, SceneService>(Lifetime.Singleton);
        }
    }
}