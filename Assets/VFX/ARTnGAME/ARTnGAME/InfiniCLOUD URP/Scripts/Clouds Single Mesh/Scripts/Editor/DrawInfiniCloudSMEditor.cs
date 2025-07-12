using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Artngame.SKYMASTER
{
    [CustomEditor(typeof(DrawInfiniCloudSM))]
    public class DrawInfiniCloudSMEditor : Editor
    {
        private DrawInfiniCloudSM clouds;

        void Awake()
        {
            clouds = (DrawInfiniCloudSM)target;
        }

        public override void OnInspectorGUI()
        {
            if (1==0 && !Application.isPlaying)
            {
                clouds.Update();
                SceneView.RepaintAll();
                if (Event.current.type == EventType.Repaint)
                {
                    SceneView.RepaintAll();
                    clouds.Update();
                }
                //if (Event.current.type == EventType.MouseDrag)
                //{
                //SceneView.RepaintAll();
                //clouds.Update();
                //}
                if (Event.current.type == EventType.MouseDown)
                {
                    SceneView.RepaintAll();
                    clouds.Update();
                }

                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

                //System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
                //System.Type type = assembly.GetType("UnityEditor.GameView");
                //EditorWindow gameview = EditorWindow.GetWindow(type);
                //gameview.Repaint();

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(clouds);
                    serializedObject.ApplyModifiedProperties();
                }
                Undo.RegisterCompleteObjectUndo(clouds, "cloud register");
            }
            DrawDefaultInspector();
        }
    }
}
