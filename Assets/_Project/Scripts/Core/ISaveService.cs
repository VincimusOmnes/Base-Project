using System;

namespace Marmalade.Core
{
    /// <summary>
    /// Manages persistent game progress. Holds save data in memory and
    /// provides explicit load and save operations backed by PlayerPrefs.
    /// Implementations should load once on startup via Initialize().
    /// </summary>
    public interface ISaveService
    {
        /// <summary>
        /// The current in-memory save data. Always reflects the last loaded
        /// or saved state. Modify this directly then call Save() to persist.
        /// </summary>
        SaveData Data { get; }

        /// <summary>
        /// Whether a save file exists in persistent storage.
        /// </summary>
        bool HasSave { get; }

        /// <summary>
        /// Loads save data from persistent storage into memory.
        /// If no save exists, Data is initialised with default values.
        /// Implementations should call this once during Initialize().
        /// </summary>
        void Load();

        /// <summary>
        /// Writes the current in-memory Data to persistent storage.
        /// Publishes GameSavedMessage on success.
        /// </summary>
        void Save();

        /// <summary>
        /// Erases all save data from persistent storage and resets
        /// Data to default values. Publishes SaveDeletedMessage.
        /// </summary>
        void Delete();
    }
}