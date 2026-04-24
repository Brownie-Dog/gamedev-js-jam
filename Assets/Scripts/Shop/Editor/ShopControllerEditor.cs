using Shop;
using UnityEditor;
using UnityEngine;

namespace Shop.Editor
{
    [CustomEditor(typeof(ShopController))]
    public class ShopControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Restock All", GUILayout.Height(30)))
            {
                ((ShopController)target).RestockAll();
            }
        }
    }
}