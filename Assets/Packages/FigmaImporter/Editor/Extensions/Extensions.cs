using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FigmaImporter.Editor {
    public class Extensions {

        [MenuItem("Tools/FigmaImporter/Create All Settings")]
        public static void CreateAllSettings() {
            FigmaImporterSettings.GetInstance();
            FontLinks.GetInstance();
            GradientsGenerator.GetInstance();
        }

        [MenuItem("Tools/FigmaImporter/Extensions/Cleanup")]
        public static void CleanupFirstTime() {
            for (var i = 0; i < 3; i++) {
                Cleanup();
            }

            PivotCenter.PivotCenterRecursive();
            AutoAnchor.AnchorCornerRecursive();
        }

        public static void Cleanup() {
            AutoSizeTMPs();
            RemoveEmptyImages(Selection.gameObjects);
            DestroyEmptyObjects(Selection.gameObjects);
            MoveOneChildToParent(Selection.gameObjects);
            CopyRectToPrefabs(Selection.gameObjects);
        }

        public static void AutoSizeTMPs(params GameObject[] gameObjects) {
            var tmps = gameObjects.SelectMany(go => go.GetComponentsInChildren<TextMeshProUGUI>(true));
            foreach (var tmp in tmps) {
                // as the size we want
                if (tmp.fontSizeMax < tmp.fontSize + 0.1f) continue;

                // then change size of rect transform
                var rect = tmp.rectTransform;

                // if anchor full parent's Y
                if (Mathf.Abs(rect.anchorMax.y - rect.anchorMin.y) > 0.6f) continue;
                // or
                if (rect.sizeDelta.y == 0) continue;

                // increase heigh
                var sizeDelta = rect.sizeDelta;
                sizeDelta = new Vector2(sizeDelta.x, Mathf.RoundToInt(tmp.fontSizeMax * 1.111f));
                rect.sizeDelta = sizeDelta;
                Debug.Log($"Auto Size TMP: {tmp.name}");
            }
        }

        public static void CopyRectToPrefabs(params GameObject[] gameObjects) {
            var children = gameObjects.SelectMany(go => go.GetComponentsInChildren<RectTransform>(true));
            foreach (var child in children) {
                var isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(child.gameObject);
                if (!isPrefab) continue;
                var nextIndex = child.GetSiblingIndex() + 1;
                if (!child.transform.parent) continue;
                if (child.transform.parent.childCount <= nextIndex) continue;
                var next = child.transform.parent.GetChild(nextIndex);
                if (next.name != child.name) continue;
                UnityEditorInternal.ComponentUtility.CopyComponent(next);
                UnityEditorInternal.ComponentUtility.PasteComponentValues(child);
                Debug.Log($"Copy Rect To Prefab: {next.name}");
            }
        }

        public static void MoveOneChildToParent(params GameObject[] gameObjects) {
            var rects = gameObjects.SelectMany(go => go.GetComponentsInChildren<RectTransform>(true));

            // clean One child full of Parent
            foreach (var rect in rects) {
                if (rect.parent == null) continue;
                if (rect.parent.childCount != 1) continue;

                if (PrefabUtility.IsPartOfAnyPrefab(rect.gameObject)) continue;

                if (!HasNoComponent(rect.parent)) continue;

                // move child to parent of parent
                var parent = rect.parent;
                rect.SetParent(parent.parent);
                rect.SetSiblingIndex(parent.GetSiblingIndex());
                AutoAnchor.AnchorCorner(rect);

                // destroy parent
                Object.DestroyImmediate(parent.gameObject);

                Debug.Log($"<color=red>Move</color> 1 Child [{rect.name}] to Parent: [{rect.parent.name}]");
            }
        }

        public static void DestroyEmptyObjects(params GameObject[] gameObjects) {
            var transforms = gameObjects.SelectMany(go => go.GetComponentsInChildren<Transform>(true));
            foreach (var t in transforms) {
                if (t.childCount > 0) continue;
                if (!HasNoComponent(t)) continue;
                if (PrefabUtility.IsPartOfAnyPrefab(t)) continue;
                Debug.Log($"<color=red>Destroy</color> empty GameObject: {t.name}");
                Object.DestroyImmediate(t.gameObject);
            }
        }

        private static bool HasNoComponent(Component t) {
            var components = t.gameObject.GetComponents<Component>().ToList();
            components.RemoveAll(x => x is RectTransform or CanvasRenderer);
            return components.Count == 0;
        }

        public static void RemoveEmptyImages(params GameObject[] gameObjects) {
            var images = gameObjects.SelectMany(go => go.GetComponentsInChildren<Image>(true));
            foreach (var image in images) {
                // if (image.enabled) continue;
                if (image.sprite != null) continue;
                Debug.Log($"<color=red>Remove</color> Empty Image: {image.name}");
                Object.DestroyImmediate(image);
            }
        }
    }
}
