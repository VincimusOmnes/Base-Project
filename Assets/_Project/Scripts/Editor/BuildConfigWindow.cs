#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;

namespace Marmalade.Editor
{
    /// <summary>
    /// Editor window for managing scripting define symbols for the WebGL build target.
    /// Provides a simple checkbox UI for toggling ENABLE_LOGGING, ENABLE_DEV_TOOLS, and ENABLE_CHEATS
    /// without needing to manually edit the raw define string in Player Settings.
    /// UNITASK_WEBGL_THREADING_SUPPORT is always included and cannot be toggled.
    /// Access via Marmalade -> Build Config in the Unity menu bar.
    /// </summary>
    public class BuildConfigWindow : EditorWindow
    {
        /// <summary>
        /// Opens the Build Config window. Accessible via Marmalade -> Build Config in the menu bar.
        /// </summary>
        [MenuItem("Marmalade/Build Config")]
        public static void Open()
        {
            GetWindow<BuildConfigWindow>("Build Config");
        }

        private void OnGUI()
        {
            NamedBuildTarget platform = NamedBuildTarget.WebGL;
            string defines = PlayerSettings.GetScriptingDefineSymbols(platform);

            bool logging = DrawToggle("ENABLE_LOGGING", defines);
            bool devTools = DrawToggle("ENABLE_DEV_TOOLS", defines);
            bool cheats = DrawToggle("ENABLE_CHEATS", defines);

            string newDefines = BuildDefineString(logging, devTools, cheats);
            if (newDefines != defines)
            {
                PlayerSettings.SetScriptingDefineSymbols(platform, newDefines);
            }
        }

        private bool DrawToggle(string symbol, string defines)
        {
            return EditorGUILayout.Toggle(symbol, defines.Contains(symbol));
        }

        private string BuildDefineString(bool logging, bool devTools, bool cheats)
        {
            System.Collections.Generic.List<string> symbols = new();
            if (logging) symbols.Add("ENABLE_LOGGING");
            if (devTools) symbols.Add("ENABLE_DEV_TOOLS");
            if (cheats) symbols.Add("ENABLE_CHEATS");
            symbols.Add("UNITASK_WEBGL_THREADING_SUPPORT");
            return string.Join(";", symbols);
        }
    }
}
#endif