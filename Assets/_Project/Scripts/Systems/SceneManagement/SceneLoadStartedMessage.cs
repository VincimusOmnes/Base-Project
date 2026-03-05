namespace Marmalade.Systems
{
    /// <summary>
    /// Published by SceneService when a scene load operation begins.
    /// Subscribe to this message to show loading UI or prepare systems for a scene transition.
    /// </summary>
    public readonly struct SceneLoadStartedMessage
    {
        /// <summary>
        /// The name of the scene being loaded.
        /// </summary>
        public readonly string SceneName;

        public SceneLoadStartedMessage(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}