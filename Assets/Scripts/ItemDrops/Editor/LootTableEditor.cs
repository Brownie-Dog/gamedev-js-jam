using System.Collections.Generic;
using ItemDrops;
using UnityEditor;
using UnityEngine;

namespace ItemDrops.Editor
{
    [CustomEditor(typeof(LootTable))]
    public class LootTableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Populate With All Items", GUILayout.Height(30)))
            {
                PopulateItems();
            }

            if (GUILayout.Button("Clear All Items", GUILayout.Height(30)))
            {
                ClearItems();
            }
        }

        private void PopulateItems()
        {
            var lootTable = (LootTable)target;
            var allItems = FindAllItemData();
            var existingItems = new HashSet<ItemData>();

            var so = new SerializedObject(lootTable);
            var itemsProperty = so.FindProperty("_items");

            for (int i = 0; i < itemsProperty.arraySize; i++)
            {
                var entry = itemsProperty.GetArrayElementAtIndex(i);
                var itemProp = entry.FindPropertyRelative("_item");
                if (itemProp != null && itemProp.objectReferenceValue is ItemData existing)
                {
                    existingItems.Add(existing);
                }
            }

            int added = 0;
            foreach (var item in allItems)
            {
                if (existingItems.Contains(item))
                {
                    continue;
                }

                itemsProperty.arraySize++;
                var newEntry = itemsProperty.GetArrayElementAtIndex(itemsProperty.arraySize - 1);

                var newItemProp = newEntry.FindPropertyRelative("_item");
                var newWeightProp = newEntry.FindPropertyRelative("_weight");

                if (newItemProp != null)
                {
                    newItemProp.objectReferenceValue = item;
                }

                if (newWeightProp != null)
                {
                    newWeightProp.floatValue = 1.0f;
                }

                added++;
            }

            so.ApplyModifiedProperties();

            if (added > 0)
            {
                EditorUtility.SetDirty(lootTable);
                Debug.Log($"Added {added} items to LootTable.");
            }
            else
            {
                Debug.Log("No new items to add.");
            }
        }

        private void ClearItems()
        {
            var lootTable = (LootTable)target;
            var so = new SerializedObject(lootTable);
            var itemsProperty = so.FindProperty("_items");
            itemsProperty.arraySize = 0;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(lootTable);
        }

        private List<ItemData> FindAllItemData()
        {
            var items = new List<ItemData>();
            var guids = AssetDatabase.FindAssets($"t:{nameof(ItemData)}");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
                if (item != null)
                {
                    items.Add(item);
                }
            }

            return items;
        }
    }
}
