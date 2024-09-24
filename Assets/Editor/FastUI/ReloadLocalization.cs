using Axie;
using AxieGameKit;
using UnityEditor;
using UnityEngine;

public class ReloadLocalization {
    [MenuItem("Tools/FastUI/Reload Localization")]
    public static void Reload() {
        if (!Application.isPlaying) return;
        var jobs = new SequenceJobs();
        jobs.Add(new LoadLocalizeJob());
        jobs.OnCompleted += OnCompletedReloadLocalization;
        jobs.Start();
    }

    private static void OnCompletedReloadLocalization(Job obj) {
        Debug.Log("<color=green>Reload Localization DONE</color>");
    }
}
