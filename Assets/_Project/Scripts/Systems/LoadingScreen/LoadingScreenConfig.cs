using UnityEngine;

namespace Marmalade.Systems
{
    [CreateAssetMenu(fileName = "LoadingScreenConfig", menuName = "Marmalade Objects/LoadingScreenConfig")]
    /// <summary>
    /// Configuration asset for the loading screen system.
    /// A single instance of this asset should exist in the project under _Project/Settings/
    /// and be assigned to the BootstrapLifetimeScope in the Bootstrap scene Inspector.
    /// Tweak values here to adjust loading screen behaviour per project.
    /// </summary>
    public class LoadingScreenConfig : ScriptableObject
    {
        /// <summary>
        /// The minimum time in seconds the loading screen is displayed regardless of load speed.
        /// Prevents the loading screen from flashing on fast loads.
        /// </summary>
        [field: SerializeField] public float MinimumDisplaySeconds { get; private set; } = 1.5f;
        /// <summary>
        /// The speed at which the loading screen fades in.
        /// </summary>
        [field: SerializeField] public float FadeInDuration { get; private set; } = 0.3f;
        /// <summary>
        /// The speed at whcih the loading screen fades out.
        /// </summary>
        [field: SerializeField] public float FadeOutDuration { get; private set; } = 0.3f;
        /// <summary>
        /// The duration of the smoothing animation between ticks up in progress. Higher = smoother but less accurate 
        /// </summary>
        [field: SerializeField] public float ProgressSmoothing { get; private set; } = 0.15f;
    
    }
}