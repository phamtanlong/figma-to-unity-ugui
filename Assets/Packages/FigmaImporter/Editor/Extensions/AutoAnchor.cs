using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FigmaImporter.Editor {
    public class AutoAnchor {
        [MenuItem("Tools/FigmaImporter/Extensions/Anchor Corner Recursive")]
        public static void AnchorCornerRecursive() {
            AnchorCornerRecursive(Selection.gameObjects);
        }

        public static void AnchorCornerRecursive(params GameObject[] gameObjects) {
            var rectTransforms = gameObjects.SelectMany(go => go.GetComponentsInChildren<RectTransform>(true));
            foreach (var rect in rectTransforms) {
                if (rect.name.Contains("@NoScale")) continue;

                // ignore prefabs in Elements folder
                var isPrefab = PrefabUtility.IsPartOfAnyPrefab(rect.gameObject);
                if (isPrefab) continue;

                AnchorCorner(rect);
            }
        }

        public static void AnchorCorner(RectTransform rect) {
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

        // this function set fullscreen and change value of anchor
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
