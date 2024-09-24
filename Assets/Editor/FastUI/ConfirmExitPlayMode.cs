#if FAST_UI
using UnityEditor;

[InitializeOnLoad]
public class ConfirmExitPlayMode {
    static ConfirmExitPlayMode() {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange stateChange) {
        // Check if we are about to exit Play mode
        if (stateChange != PlayModeStateChange.ExitingPlayMode) return;
        // Show confirmation dialog
        if (!EditorUtility.DisplayDialog("Confirm Exit", "Exit Play mode?", "Yes", "No")) {
            // Cancel the Play mode exit
            EditorApplication.isPlaying = true;
        }
    }
}
#endif
