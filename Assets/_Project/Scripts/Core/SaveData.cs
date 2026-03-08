namespace Marmalade.Core
{
    /// <summary>
    /// Serializable container for persistent game progress data.
    /// Extend or replace this class in your feature assemblies to add
    /// game-specific fields. Keep all properties JSON-serializable primitives
    /// or nested serializable classes — no Unity types.
    /// </summary>
    public class SaveData
    {
        public float TotalPlayTimeSeconds { get; set; }
        public string LastSavedUtc        { get; set; } = string.Empty;
    }
}
