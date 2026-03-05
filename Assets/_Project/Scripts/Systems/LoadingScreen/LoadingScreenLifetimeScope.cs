using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Marmalade.Systems
{
    /// <summary>
    /// VContainer scope for the LoadingScreen scene.
    /// Child of BootstrapLifetimeScope
    /// </summary>
    public class LoadingScreenLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<LoadingScreenView>();
            builder.RegisterEntryPoint<LoadingScreenPresenter>();
        }
    }
}