using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FigmaImporter.Editor {
    public class Tools {
        [MenuItem("Tools/FigmaImporter/AA FIRST-TIME")]
        public static void FirstTime() {
            CleanupMany();
            PivotCenters();
            AutoCornerRecursive();
        }

        [MenuItem("Tools/FigmaImporter/Cleanup MANY")]
        public static void CleanupMany() {
            for (var i = 0; i < 5; i++) {
                Cleanup();
            }
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

        [MenuItem("Tools/FigmaImporter/Move One Child To Parent")]
        public static void MoveOneChildToParent() {
            MoveOneChildToParent(Selection.gameObjects);
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
                AutoCorner(rect);

                // destroy parent
                Object.DestroyImmediate(parent.gameObject);

                Debug.Log($"<color=red>Move</color> 1 Child [{rect.name}] to Parent: [{rect.parent.name}]");
            }
        }

        private static bool Equals(Vector3 v1, Vector3 v2) {
            return Mathf.Abs((v1 - v2).magnitude) < 0.2f;
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

        [MenuItem("Tools/FigmaImporter/Pivot Centers")]
        public static void PivotCenters() {
            PivotCenters(Selection.gameObjects);
        }

        public static void PivotCenters(params GameObject[] gameObjects) {
            var rectTransforms = gameObjects.SelectMany(go => go.GetComponentsInChildren<RectTransform>(true));
            foreach (var rect in rectTransforms) {
                ChangePivot(rect, Vector2.one * 0.5f);
            }
        }

        public static void ChangePivot(RectTransform rect, Vector2 pivot) {
            var anchorPos = rect.anchoredPosition;
            var sizeDelta = rect.sizeDelta;
            var deltaPivot = pivot - rect.pivot;
            var newAnchorPos = anchorPos + (deltaPivot * sizeDelta);
            rect.pivot = pivot;
            rect.anchoredPosition = newAnchorPos;
        }

        [MenuItem("Tools/FigmaImporter/Auto Corner Recursive")]
        public static void AutoCornerRecursive() {
            AutoCorners(Selection.gameObjects);
        }

        public static void AutoCorners(params GameObject[] gameObjects) {
            var rectTransforms = gameObjects.SelectMany(go => go.GetComponentsInChildren<RectTransform>(true));
            foreach (var rect in rectTransforms) {
                if (rect.name.Contains("@NoScale")) continue;

                // ignore prefabs in Elements folder
                var isPrefab = PrefabUtility.IsPartOfAnyPrefab(rect.gameObject);
                if (isPrefab) continue;

                AutoCorner(rect);
            }
        }

        public static void AutoCorner(RectTransform rect) {
            AnchorsToCorners(rect);

            (Vector2, Vector2) run(Vector2 min, Vector2 max) {
                var anchorMin = new Vector2(min.x, min.y) * 2;
                var anchorMax = new Vector2(max.x, max.y) * 2;
                anchorMin = new Vector2(Mathf.RoundToInt(anchorMin.x), Mathf.RoundToInt(anchorMin.y));
                anchorMax = new Vector2(Mathf.RoundToInt(anchorMax.x), Mathf.RoundToInt(anchorMax.y));
                anchorMin /= 2f;
                anchorMax /= 2f;
                return (anchorMin, anchorMax);
            }

            var min = rect.anchorMin;
            var max = rect.anchorMax;

            while (true) {
                var (resMin, resMax) = run(min, max);

                var absX = Mathf.Abs(resMax.x - resMin.x);
                if (absX is > 0.1f and < 0.9f) {
                    min.x = (min.x + max.x) / 2f;
                    max.x = min.x;
                    continue;
                }

                var absY = Mathf.Abs(resMax.y - resMin.y);
                if (absY is > 0.1f and < 0.9f) {
                    min.y = (min.y + max.y) / 2f;
                    max.y = min.y;
                    continue;
                }

                SetAnchors(rect, resMin, resMax);
                break;
            }
        }

        public static void AnchorsToCorners(RectTransform t) {
            if (t == null) return;
            var pt = t.parent as RectTransform;
            if (pt == null) return;

            var newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                t.anchorMin.y + t.offsetMin.y / pt.rect.height);
            var newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                t.anchorMax.y + t.offsetMax.y / pt.rect.height);

            t.anchorMin = newAnchorsMin;
            t.anchorMax = newAnchorsMax;
            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }

        public static void SetAnchors(RectTransform rectTransform, Vector2 minAnchor, Vector2 maxAnchor) {
            var parentTransform = (RectTransform)rectTransform.parent;
            var offsetMin = rectTransform.offsetMin;
            var offsetMax = rectTransform.offsetMax;
            var parentSize = parentTransform.rect.size;
            var positionMin = Vector2.Scale(rectTransform.anchorMin, parentSize) + offsetMin;
            var positionMax = Vector2.Scale(rectTransform.anchorMax, parentSize) + offsetMax;

            rectTransform.anchorMin = minAnchor;
            rectTransform.anchorMax = maxAnchor;

            rectTransform.offsetMin = positionMin - Vector2.Scale(rectTransform.anchorMin, parentSize);
            rectTransform.offsetMax = positionMax - Vector2.Scale(rectTransform.anchorMax, parentSize);
        }
    }
}
