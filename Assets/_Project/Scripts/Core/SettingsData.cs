using System.Collections.Generic;

namespace Marmalade.Core
{
    /// <summary>
    /// Serializable container for persistent user preferences.
    /// Extend or replace this class in your feature assemblies to add
    /// game-specific settings. Keybindings are stored as a dictionary of
    /// action name to KeyCode value, allowing games to define their own
    /// input actions without modifying this class.
    /// </summary>
    public class SettingsData
    {
        public float MasterVolume { get; set; } = 1f;
        public float MusicVolume  { get; set; } = 1f;
        public float SfxVolume    { get; set; } = 1f;

        /// <summary>
        /// Maps input action names to their bound KeyCode integer values.
        /// KeyCode is stored as int for JSON compatibility.
        /// Example entry: { "Jump", 32 } where 32 is KeyCode.Space.
        /// </summary>
        public Dictionary<string, int> Keybindings { get; set; } = new();
    }
}
