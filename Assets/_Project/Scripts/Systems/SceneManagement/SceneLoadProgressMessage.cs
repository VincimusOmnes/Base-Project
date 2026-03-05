namespace Marmalade.Systems
{
    /// <summary>
    /// Published by SceneService repeatedly during a scene load operation.
    /// Use the Progress value to update a loading bar or other progress indicator.
    /// </summary>
    public readonly struct SceneLoadProgressMessage
    {
        /// <summary>
        /// The current load progress as a normalised value between 0 and 1.
        /// </summary>
        public readonly float Progress;

        public SceneLoadProgressMessage(float progress)
        {
            Progress = progress;
        }
    }
}