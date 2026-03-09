using UnityEngine;
using UnityEngine.Audio;

namespace Marmalade.Systems
{
    /// <summary>
    /// Configuration asset for the audio system. Holds a reference to the
    /// AudioMixer and pool settings. Assign the MarmaladeAudioMixer asset
    /// and configure pool size in the Inspector.
    /// Lives in _Project/Settings/ — designers own the values, programmers
    /// own the code.
    /// </summary>
    [CreateAssetMenu(menuName = "Marmalade Objects/AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        /// <summary>The AudioMixer asset containing Master, Music, and SFX groups.</summary>
        [field: SerializeField] public AudioMixer AudioMixer { get; private set; }

        /// <summary>The AudioMixer group for SFX</summary>
        [field: SerializeField] public AudioMixerGroup SfxMixerGroup { get; private set; }

        /// <summary>
        /// Number of AudioSource pool slots reserved for SFX playback.
        /// If all slots are busy when PlaySfx is called, the sound is dropped
        /// and a warning is logged. Increase this value if warnings appear.
        /// </summary>
        [field: SerializeField] public int SfxPoolSize { get; private set; } = 16;

        /// <summary>The exposed parameter name for master volume on the AudioMixer.</summary>
        public const string MasterVolumeParam = "MasterVolume";

        /// <summary>The exposed parameter name for music volume on the AudioMixer.</summary>
        public const string MusicVolumeParam = "MusicVolume";

        /// <summary>The exposed parameter name for SFX volume on the AudioMixer.</summary>
        public const string SfxVolumeParam = "SfxVolume";

        // ── UI Sounds ─────────────────────────────────────────────────────────

        [Header("UI Sounds")]
        [field: SerializeField] public AudioClip ButtonClick { get; private set; }
        [field: SerializeField] public AudioClip LoadingStarted { get; private set; }
        [field: SerializeField] public AudioClip LoadingCompleted { get; private set; }
    }
}