#if FAST_UI
using System.IO;
using AxieGameKit;
using UIManagement;
using UnityEngine;
using UnityEditor;

public class PrefabSaveProcessor : AssetPostprocessor {
    private static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths) {
        if (!EditorApplication.isPlaying) return;
        foreach (var asset in importedAssets) {
            if (!asset.EndsWith(".prefab")) continue;
            // Your logic when a prefab is saved
            Debug.Log($"<color=green>Prefab Saved</color>: [{asset}] > Refresh Components");
            Runner.RunAction(0.3f, () => RefreshStep1(asset));
        }
    }

    private static void RefreshStep1(string asset) {
        var viewName = Path.GetFileNameWithoutExtension(asset);

        // BaseUIView
        var uiViews = Object.FindObjectsOfType<BaseUIView>();
        foreach (var view in uiViews) {
            if (view.name != viewName) continue;
            view.SafeActive(false);
            view.SendMessage("Awake", null, SendMessageOptions.DontRequireReceiver);
            Runner.RunAction(0.2f, () => RefreshStep2(view));
        }
    }

    private static void RefreshStep2(Component view) {
        view.SafeActive(true);
        view.SendMessage("Start", null, SendMessageOptions.DontRequireReceiver);
        view.SendMessage("OnShown", null, SendMessageOptions.DontRequireReceiver);
    }
}
#endif
