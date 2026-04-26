using Enemy.Bosses;
using UnityEditor;
using UnityEngine;

namespace Enemy.Bosses.Editor
{
    [CustomEditor(typeof(OvenBossArmSpawner))]
    public class OvenBossArmSpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Refresh Arms", GUILayout.Height(30)))
            {
                ((OvenBossArmSpawner)target).RefreshArms();
            }
        }
    }
}