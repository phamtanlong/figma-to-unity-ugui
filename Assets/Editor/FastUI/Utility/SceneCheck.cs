using UnityEngine;

public class SceneCheck : MonoBehaviour {
    public static bool IsInPrefabEditMode(GameObject obj) {
        // Check if the GameObject is in Prefab Mode
        var prefabStage =
            UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(obj);
        if (prefabStage != null) {
            // The GameObject is in the Prefab Edit Scene
            return true;
        }

        // The GameObject is not in Prefab Edit Mode
        return false;
    }

    public static bool IsInLoadedScene(GameObject obj) {
        // Get the scene the GameObject belongs to
        var scene = obj.scene;

        // Check if the scene is loaded and is not empty
        if (scene.isLoaded && !string.IsNullOrEmpty(scene.name)) {
            // The GameObject is in the loaded scene
            return true;
        }

        return false;
    }
}
