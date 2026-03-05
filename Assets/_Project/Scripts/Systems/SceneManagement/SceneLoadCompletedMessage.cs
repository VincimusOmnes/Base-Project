namespace Marmalade.Systems
{
    /// <summary>
    /// Published by SceneService when a scene load operation has completed successfully.
    /// Subscribe to this message to hide loading UI or initialise systems after a scene transition.
    /// </summary>
    public readonly struct SceneLoadCompletedMessage
    {
        /// <summary>
        /// The name of the scene that finished loading.
        /// </summary>
        public readonly string SceneName;

        public SceneLoadCompletedMessage(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}