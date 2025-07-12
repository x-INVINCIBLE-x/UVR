using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Artngame.CommonTools
{
    public class sceneSwitcherEditorTimeSM : EditorWindow
    {
        [MenuItem("Tools/ARTnGAME/Scene Switcher")]
        static void Init()
        {
            sceneSwitcherEditorTimeSM window = (sceneSwitcherEditorTimeSM)GetWindow(typeof(sceneSwitcherEditorTimeSM));
            window.Show();
        }

        void OnGUI()
        {
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (GUILayout.Button(System.IO.Path.GetFileNameWithoutExtension(scene.path)))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scene.path);
                    }
                }
            }
        }
    }
}
