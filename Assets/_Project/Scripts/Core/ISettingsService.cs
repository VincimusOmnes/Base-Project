namespace Marmalade.Core
{
    /// <summary>
    /// Manages persistent user preferences such as audio volumes and display
    /// settings. Holds settings in memory and writes to persistent storage
    /// immediately on every change. Implementations should load once on
    /// startup via Initialize(), before any systems that depend on settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// The current in-memory settings. Always reflects the last loaded
        /// or saved state. Modify this directly then call Save() to persist.
        /// </summary>
        SettingsData Data { get; }

        /// <summary>
        /// Loads settings from persistent storage into memory.
        /// If no settings exist, Data is initialised with default values.
        /// Implementations should call this once during Initialize().
        /// </summary>
        void Load();

        /// <summary>
        /// Writes the current in-memory Data to persistent storage.
        /// Publishes SettingsChangedMessage on success.
        /// </summary>
        void Save();

        /// <summary>
        /// Resets Data to default values and persists immediately.
        /// Publishes SettingsChangedMessage.
        /// </summary>
        void ResetToDefaults();
    }
}