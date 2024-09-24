#if FAST_UI
using System.Linq;
using Axie;
using TMPro;
using UIManagement;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[InitializeOnLoad]
public static class DecorateComponents {
    static DecorateComponents() {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private static void OnHierarchyChanged() {
        foreach (var gameObject in Selection.gameObjects) {
            Process(gameObject);
        }
    }

    private static void Process(GameObject obj) {
        if (obj == null) return;
        if (obj.TryGetComponent<ScrollRect>(out var scrollRect)) {
            ProcessScrollRect(scrollRect);
        }

        if (obj.TryGetComponent<TooltipSource>(out var source)) {
            ProcessTooltipSource(source);
        }

        if (obj.TryGetComponent<Selectable>(out var selectable)) {
            ProcessSelectable(selectable);
        }

        if (obj.TryGetComponent<Button>(out var button)) {
            ProcessButton(button);
        }
    }

    private static void ProcessSelectable(Selectable com) {
        // add hover zoom
        if (com is not TMP_InputField && com is not Toggle) {
            var hoverZoom = com.GetComponent<HoverZoom>();
            if (hoverZoom == null) {
                com.GetOrAddComponent<HoverZoom>();
                Debug.Log($"<color=green>{com.GetType().Name}</color> [{com.name}] added. Add HoverZoom");
            }
        }

        // set target graphic
        if (com.targetGraphic == null) {
            var graphic = com.GetComponentsInChildren<Graphic>().FirstOrDefault();
            com.targetGraphic = graphic;
            Debug.Log($"<color=green>{com.GetType().Name}</color> [{com.name}] added. Set targetGraphic");
        }
    }

    private static void ProcessTooltipSource(TooltipSource com) {
        com.GetOrAddComponent<HoverZoom>();
        Debug.Log($"<color=green>{com.GetType().Name}</color> [{com.name}] added. Add HoverZoom");
    }

    private static void ProcessScrollRect(ScrollRect com) {
        if (com.scrollSensitivity < 10) {
            com.scrollSensitivity = 10;
            Debug.Log($"<color=green>{com.GetType().Name}</color> [{com.name}] added. scrollSensitivity = 10");
        }

        if (com.verticalScrollbar != null) {
            Object.DestroyImmediate(com.verticalScrollbar.gameObject);
            Debug.Log($"<color=green>{com.GetType().Name}</color> [{com.name}] added. destroy verticalScrollbar");
        }

        if (com.horizontalScrollbar != null) {
            Object.DestroyImmediate(com.horizontalScrollbar.gameObject);
            Debug.Log($"<color=green>{com.GetType().Name}</color> [{com.name}] added. destroy horizontalScrollbar");
        }

        if (com.viewport.sizeDelta != Vector2.zero) {
            com.viewport.sizeDelta = Vector2.zero;
            Debug.Log($"<color=green>{com.GetType().Name}</color> [{com.name}] added. viewPort full parent");

            var graphic = com.GetComponent<Graphic>();
            if (graphic) graphic.color = new Color(1, 1, 1, 0.004f);
            Debug.Log($"<color=green>{com.GetType().Name}</color> [{com.name}] added. reduce graphic alpha");
        }

        if (com.vertical) {
            var fitter = com.content.GetOrAddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

        if (com.horizontal) {
            var fitter = com.content.GetOrAddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    private static void ProcessButton(Button button) {
        // add listener
        var count = button.onClick.GetPersistentEventCount();
        if (count == 0) {
            var behaviours = button.GetComponentsInParent<MonoBehaviour>().ToList();

            foreach (var behaviour in behaviours) {
                var fullName = behaviour.GetType().FullName;
                if (fullName != null && fullName.Contains("UnityEngine")) continue;
                ButtonUtility.AddButtonListener(button, behaviour);
                if (behaviour is BaseUIView) break; // stop here
            }

            EditorUtility.SetDirty(button);
        }

        // more ...
    }
}
#endif
