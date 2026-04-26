using UnityEngine;
using UnityEditor;

namespace Enemy.Bosses.Editor
{
    [CustomEditor(typeof(FanManFanPushMove))]
    public class FanManFanPushMoveEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

            var move = (FanManFanPushMove)target;

            if (GUILayout.Button("Force Startup Trigger"))
            {
                move.ForceStartupTrigger();
            }

            if (GUILayout.Button("Force Push Bool (true)"))
            {
                move.ForcePushBool(true);
            }

            if (GUILayout.Button("Force Push Bool (false)"))
            {
                move.ForcePushBool(false);
            }
        }
    }
}
