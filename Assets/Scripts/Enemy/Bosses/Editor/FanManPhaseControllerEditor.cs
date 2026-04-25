using UnityEngine;
using UnityEditor;

namespace Enemy.Bosses.Editor
{
    [CustomEditor(typeof(FanManPhaseController))]
    public class FanManPhaseControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

            var controller = (FanManPhaseController)target;

            if (GUILayout.Button("Force Phase 1"))
            {
                controller.ForcePhase(FanManPhaseController.Phase.One);
            }

            if (GUILayout.Button("Force Phase 2"))
            {
                controller.ForcePhase(FanManPhaseController.Phase.Two);
            }

            if (GUILayout.Button("Force Phase 3"))
            {
                controller.ForcePhase(FanManPhaseController.Phase.Three);
            }

            if (GUILayout.Button("Unlock Phase (Health-Based)"))
            {
                controller.UnlockPhase();
            }
        }
    }
}
