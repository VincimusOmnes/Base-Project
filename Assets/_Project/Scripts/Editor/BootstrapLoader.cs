using UnityEngine;
using UnityEngine.SceneManagement;

namespace Marmalade.Editor
{
    /// <summary>
    /// Ensures Bootstrap is always the first scene to run in the Editor, regardless
    /// of which scene is open when Play is pressed.
    /// </summary>
    public static class BootstrapLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadBootstrap()
        {
            if (SceneManager.GetActiveScene().name == "Bootstrap")
                return;

            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
        }
    }
}