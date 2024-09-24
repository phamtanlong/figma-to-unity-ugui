using System.IO;
using UnityEditor;
using UnityEngine;

namespace FigmaImporter.Editor {
    public partial class FigmaNodeGenerator {
        private static void LoadPreset(Node node, GameObject parent) {
            var objectName = node.objectName();

            var folder = FigmaImporterSettings.GetInstance().PresetsPath;
            var path = $"Assets/{folder}/{objectName}.prefab";
            if (!File.Exists(path)) return;

            // // option 1
            // if (node.children.Length < 2) return;
            // if (node.children[0].name != node.children[1].name) return;

            // // option 2
            // if (node.type != "INSTANCE") return;

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var nodeGo = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent.transform);
            nodeGo.SetActive(node.visible);
            Debug.Log($"Preset apply: {nodeGo.name}, parent = {parent.name}");
        }
    }
}
