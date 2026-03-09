using System;
using Marmalade.Core;
using MessagePipe;
using UnityEngine;
using UnityEngine.Audio;
using Marmalade.Shared;

namespace Marmalade.Systems
{
    /// <summary>
    /// Implements IAudioService using Unity's AudioMixer and a pool of AudioSources.
    /// AudioSource components are provided by AudioSceneInstaller from the persistent
    /// Audio scene rather than created at runtime, keeping scene hierarchy organised.
    /// Volumes are initialised from ISettingsService on startup and persisted back
    /// to ISettingsService when changed.
    /// </summary>
    public class AudioService : IAudioService
    {
        private readonly AudioConfig _config;
        private readonly ISettingsService _settingsService;
        private readonly IPublisher<MusicStartedMessage> _musicStartedPublisher;
        private readonly IPublisher<MusicStoppedMessage> _musicStoppedPublisher;

        private AudioSource _musicSource;
        private AudioSource[] _sfxPool;

        public AudioService(
            AudioConfig config,
            ISettingsService settingsService,
            IPublisher<MusicStartedMessage> musicStartedPublisher,
            IPublisher<MusicStoppedMessage> musicStoppedPublisher)
        {
            _config                = config;
            _settingsService       = settingsService;
            _musicStartedPublisher = musicStartedPublisher;
            _musicStoppedPublisher = musicStoppedPublisher;
        }

        /// <summary>
        /// Called by AudioSceneInstaller once the Audio scene has loaded.
        /// Receives the pre-configured AudioSource components from the scene
        /// and applies saved volume settings from ISettingsService.
        /// </summary>
        public void Initialize(AudioSource musicSource, AudioSource[] sfxSources)
        {
            _musicSource = musicSource;
            _sfxPool     = sfxSources;

            ApplySavedVolumes();

            Log.Info("Audio service initialised.");
        }

        // ── Music ─────────────────────────────────────────────────────────────

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            _musicSource.Stop();
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();

            _musicStartedPublisher.Publish(new MusicStartedMessage());
            Log.Info($"Playing music: {clip.name}");
        }

        public void StopMusic()
        {
            _musicSource.Stop();
            _musicStoppedPublisher.Publish(new MusicStoppedMessage());
            Log.Info("Music stopped.");
        }

        public void PauseMusic()
        {
            _musicSource.Pause();
        }

        public void ResumeMusic()
        {
            _musicSource.UnPause();
        }

        // ── SFX ───────────────────────────────────────────────────────────────

        public void PlaySfx(AudioClip clip)
        {
            AudioSource source = GetAvailableSource();

            if (source == null)
            {
                Log.Warning($"[AudioService] SFX pool exhausted — dropping sound: {clip.name}. Consider increasing SfxPoolSize in AudioConfig.");
                return;
            }

            source.clip = clip;
            source.Play();
        }

        // ── Volume ────────────────────────────────────────────────────────────

        public void SetMasterVolume(float volume)
        {
            SetMixerVolume(AudioConfig.MasterVolumeParam, volume);
            _settingsService.Data.MasterVolume = volume;
            _settingsService.Save();
        }

        public void SetMusicVolume(float volume)
        {
            SetMixerVolume(AudioConfig.MusicVolumeParam, volume);
            _settingsService.Data.MusicVolume = volume;
            _settingsService.Save();
        }

        public void SetSfxVolume(float volume)
        {
            SetMixerVolume(AudioConfig.SfxVolumeParam, volume);
            _settingsService.Data.SfxVolume = volume;
            _settingsService.Save();
        }

        // ── Private ───────────────────────────────────────────────────────────

        private void ApplySavedVolumes()
        {
            SetMixerVolume(AudioConfig.MasterVolumeParam, _settingsService.Data.MasterVolume);
            SetMixerVolume(AudioConfig.MusicVolumeParam,  _settingsService.Data.MusicVolume);
            SetMixerVolume(AudioConfig.SfxVolumeParam,    _settingsService.Data.SfxVolume);
        }

        /// <summary>
        /// Converts a 0-1 linear volume value to decibels and sets it on the mixer.
        /// AudioMixer volumes are in decibels — a value of 0.0001 is used as the
        /// minimum instead of 0 because log10(0) is undefined.
        /// </summary>
        private void SetMixerVolume(string parameter, float volume)
        {
            float db = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
            _config.AudioMixer.SetFloat(parameter, db);
        }

        /// <summary>
        /// Returns the first idle AudioSource from the SFX pool, or null if all
        /// slots are currently playing.
        /// </summary>
        private AudioSource GetAvailableSource()
        {
            foreach (AudioSource source in _sfxPool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            return null;
        }
    }
}