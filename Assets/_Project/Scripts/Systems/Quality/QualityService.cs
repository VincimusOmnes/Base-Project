using UnityEngine;
using Marmalade.Core;

namespace Marmalade.Systems
{
    /// <summary>
    /// Manages quality tier selection for WebGL builds.
    /// Detects device type at startup and sets the appropriate quality tier,
    /// and provides manual quality switching for use by the Settings screen.
    /// </summary>
    public class QualityService : IQualityService
    {
        /// <summary>
        /// Detects the current device type and sets the appropriate quality tier.
        /// Defaults to Mobile tier — upgrades to PC tier on non-handheld devices.
        /// Call once at application startup via BootstrapEntry.
        /// </summary>
        public void InitialiseQuality()
        {
            if (SystemInfo.deviceType != DeviceType.Handheld)
            {
                QualitySettings.SetQualityLevel(1); // PC tier
            }
        }

        /// <summary>
        /// Manually sets the quality tier by index.
        /// 0 = Mobile, 1 = PC.
        /// Used by the Settings screen to allow the user to override the default tier.
        /// </summary>
        public void SetQualityLevel(int level)
        {
            QualitySettings.SetQualityLevel(level);
        }
    }
}