#if SPEECH

using AxieGameKit;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class Speech {
    static Speech() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [DidReloadScripts]
    private static void DidReloadScripts() {
        Debug.Log("Assembly Compiled");
        Terminal.Run("/bin/bash", "-c \"say -v Zoe 'compile compile'\"");
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "Home") {
            Runner.RunAction(2f, () => Terminal.Run("/bin/bash", "-c \"say -v Zoe 'home home'\""));
        }
    }
}

#endif
