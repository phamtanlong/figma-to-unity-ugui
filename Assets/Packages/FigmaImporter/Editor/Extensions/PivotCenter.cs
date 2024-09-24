using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FigmaImporter.Editor {
    public class PivotCenter {
        [MenuItem("Tools/FigmaImporter/Extensions/Pivot Center Recursive")]
        public static void PivotCenterRecursive() {
            PivotCenterRecursive(Selection.gameObjects);
        }

        public static void PivotCenterRecursive(params GameObject[] gameObjects) {
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
    }
}
