using Marmalade.Core;
using Marmalade.Shared;
using UnityEngine;
using VContainer;

namespace Marmalade.Systems
{
    /// <summary>
    /// Scene-side installer for the audio system. Lives in the persistent Audio
    /// scene and owns the AudioSource components used by AudioService.
    ///
    /// AudioSource components are created programmatically at runtime based on
    /// the SfxPoolSize in AudioConfig. This keeps the hierarchy organised and
    /// ensures consistent pool sizing without manual Editor setup.
    /// </summary>
    public class AudioSceneInstaller : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicSource;

        private IAudioService _audioService;
        private AudioConfig _config;

        [Inject]
        public void Construct(IAudioService audioService, AudioConfig config)
        {
            _audioService = audioService;
            _config = config;
        }

        private void OnEnable()
        {
            if (_audioService is AudioService concreteAudioService)
            {
                AudioSource[] sfxPool = CreateSfxPool();
                concreteAudioService.Initialize(_musicSource, sfxPool);
            }
        }

        private AudioSource[] CreateSfxPool()
        {
            int poolSize = _config.SfxPoolSize;
            AudioSource[] pool = new AudioSource[poolSize];

            GameObject poolParent = new GameObject("SfxPool");
            poolParent.transform.SetParent(transform, false);

            for (int i = 0; i < poolSize; i++)
            {
                GameObject sourceObj = new GameObject($"Sfx_{i}");
                sourceObj.transform.SetParent(poolParent.transform, false);

                AudioSource source = sourceObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.Stop();
                source.loop = false;
                source.spatialBlend = 0f;
                source.outputAudioMixerGroup = _config.SfxMixerGroup;

                pool[i] = source;
            }

            Log.Info($"[AudioSceneInstaller] Created SFX pool with {poolSize} sources.");
            return pool;
        }
    }
}
