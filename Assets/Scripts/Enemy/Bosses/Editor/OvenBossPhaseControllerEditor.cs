using Enemy.Bosses;
using UnityEditor;
using UnityEngine;

namespace Enemy.Bosses.Editor
{
    [CustomEditor(typeof(OvenBossPhaseController))]
    public class OvenBossPhaseControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);

            var controller = (OvenBossPhaseController)target;

            EditorGUILayout.LabelField("Force Phase", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Phase 1", GUILayout.Height(30)))
            {
                controller.ForcePhase(OvenBossPhaseController.Phase.One);
            }
            if (GUILayout.Button("Phase 2", GUILayout.Height(30)))
            {
                controller.ForcePhase(OvenBossPhaseController.Phase.Two);
            }
            if (GUILayout.Button("Phase 3", GUILayout.Height(30)))
            {
                controller.ForcePhase(OvenBossPhaseController.Phase.Three);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Auto (Health-based)", GUILayout.Height(25)))
            {
                controller.UnlockPhase();
            }
        }
    }
}
