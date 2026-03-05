namespace Marmalade.Core
{
    /// <summary>
    /// Contract for the quality settings management service.
    /// Handles quality tier detection and switching based on device capabilities.
    /// </summary>
    public interface IQualityService
    {
        void InitialiseQuality();

        void SetQualityLevel(int level);
    }
}