using UnityEngine;

namespace Marmalade.Core
{
    /// <summary>
    /// Manages all audio playback for the application, covering music and SFX.
    /// Implementations should initialise the AudioMixer volumes from ISettingsService
    /// during Initialize().
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// Plays the given clip as the current music track.
        /// If music is already playing, stops it before starting the new clip.
        /// </summary>
        void PlayMusic(AudioClip clip, bool loop = true);

        /// <summary>
        /// Stops the current music track immediately.
        /// </summary>
        void StopMusic();

        /// <summary>
        /// Pauses the current music track at its current position.
        /// </summary>
        void PauseMusic();

        /// <summary>
        /// Resumes the current music track from its paused position.
        /// </summary>
        void ResumeMusic();

        /// <summary>
        /// Plays the given clip as a one-shot SFX using the pool.
        /// If no pool slot is available, the sound is dropped and a warning is logged.
        /// </summary>
        void PlaySfx(AudioClip clip);

        /// <summary>
        /// Sets the master volume. Value should be between 0 and 1.
        /// Implementations should persist the change via ISettingsService.
        /// </summary>
        void SetMasterVolume(float volume);

        /// <summary>
        /// Sets the music volume. Value should be between 0 and 1.
        /// Implementations should persist the change via ISettingsService.
        /// </summary>
        void SetMusicVolume(float volume);

        /// <summary>
        /// Sets the SFX volume. Value should be between 0 and 1.
        /// Implementations should persist the change via ISettingsService.
        /// </summary>
        void SetSfxVolume(float volume);
    }
}