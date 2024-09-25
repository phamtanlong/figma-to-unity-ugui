using System.IO;
using UnityEditor;
using UnityEngine;

namespace FigmaImporter.Editor {
    public partial class FigmaNodeGenerator {
        private static void LoadPreset(Node node, GameObject origin) {
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
            var nodeGo = (GameObject)PrefabUtility.InstantiatePrefab(prefab, origin.transform.parent);
            nodeGo.SetActive(node.visible);

            // copy RectTransform
            UnityEditorInternal.ComponentUtility.CopyComponent(origin.transform as RectTransform);
            UnityEditorInternal.ComponentUtility.PasteComponentValues(nodeGo.transform as RectTransform);

            Debug.Log($"Preset apply: {nodeGo.name}");
        }
    }
}
