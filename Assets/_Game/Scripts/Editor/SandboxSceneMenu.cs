using UnityEditor;
using UnityEditor.SceneManagement;

namespace Game.Editor
{
    /// <summary>Creates a personal sandbox scene from the shared template (see CONTRIBUTING §4, Scene).</summary>
    public static class SandboxSceneMenu
    {
        private const string TemplatePath = "Assets/_Game/Scenes/Sandbox/Sandbox_Template.unity";
        private const string SandboxFolder = "Assets/_Game/Scenes/Sandbox";

        [MenuItem("Game/Create My Sandbox Scene")]
        private static void CreateSandboxScene()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "New sandbox scene", "Sandbox_", "unity", "Name it after yourself, e.g. Sandbox_Ana.", SandboxFolder);
            if (string.IsNullOrEmpty(path))
                return;

            if (!AssetDatabase.CopyAsset(TemplatePath, path))
            {
                EditorUtility.DisplayDialog("Sandbox", $"Could not copy {TemplatePath}.", "OK");
                return;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(path);
        }
    }
}
