using UIManagement;
using UnityEngine;

public class DeleteCurrentUI {
    public static void Delete() {
        if (!Application.isPlaying) return;
        var baseUIView = UIManager.instance.GetCurrentView();
        if (!baseUIView) return;

        var homeCentre = baseUIView.name.Contains("HomeCentre");

        baseUIView.HideAndDestroy();

        Debug.Log($"<color=red>Delete View</color>: {baseUIView.name}");

        FastUI.Runner.RunAfterSeconds(1, () => {
            if (homeCentre) {
                UIManager.instance.Show("NewUI/HomeCentre", LayerType.Panel);
            }
        });
    }
}
