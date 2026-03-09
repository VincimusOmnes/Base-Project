using Marmalade.Systems;
using VContainer;
using VContainer.Unity;

namespace Marmalade.Bootstrap
{
    /// <summary>
    /// LifetimeScope for the persistent Audio scene. Inherits all registrations
    /// from BootstrapLifetimeScope, including IAudioService. Registers scene-local
    /// components that need injection from the parent scope.
    /// </summary>
    public class AudioLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<AudioSceneInstaller>();
        }
    }
}
