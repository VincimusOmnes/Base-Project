namespace Marmalade.Systems
{
    /// <summary>
    /// Published after save data has been successfully written to persistent storage.
    /// </summary>
    public readonly struct GameSavedMessage { }

    /// <summary>
    /// Published after save data has been successfully loaded from persistent storage.
    /// </summary>
    public readonly struct GameLoadedMessage { }

    /// <summary>
    /// Published after save data has been erased from persistent storage.
    /// </summary>
    public readonly struct SaveDeletedMessage { }

    /// <summary>
    /// Published after settings have been changed and written to persistent storage.
    /// </summary>
    public readonly struct SettingsChangedMessage { }
}