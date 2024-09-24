#if FAST_UI
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

public static class ToolbarStyles {
    public static readonly GUIStyle buttonStyle;

    static ToolbarStyles() {
        buttonStyle = new GUIStyle(GUI.skin.button) {
            // fontSize = 12,
            // alignment = TextAnchor.MiddleCenter,
            // imagePosition = ImagePosition.ImageAbove,
            // fontStyle = FontStyle.Normal,
            // fixedHeight = 20,
            // fixedWidth = 100,
        };
    }
}

[InitializeOnLoad]
public class ToolbarGUI {
    static ToolbarGUI() {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUILeft);
        ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUIRight);
    }

    private static void OnToolbarGUILeft() {
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Assign Fields", "Auto assign fields to selected object"),
            ToolbarStyles.buttonStyle)) {
            AssignFields.Run();
        }

        if (GUILayout.Button(new GUIContent("Auto Corner", "Auto Corner Recursive"),
            ToolbarStyles.buttonStyle)) {
            FigmaImporter.Editor.Tools.AutoCornerRecursive();
        }

        if (GUILayout.Button(new GUIContent("Reload Localize", "Reload all Localization/Local files"),
            ToolbarStyles.buttonStyle)) {
            ReloadLocalization.Reload();
        }

        if (GUILayout.Button(new GUIContent("Delete Current UI", "Hide current UI, then delete in scene"),
            ToolbarStyles.buttonStyle)) {
            DeleteCurrentUI.Delete();
        }
    }

    private static void OnToolbarGUIRight() {
        if (GUILayout.Button(new GUIContent("Figma 1sTime", "Figma cleanup first time"), ToolbarStyles.buttonStyle)) {
            FigmaImporter.Editor.Tools.FirstTime();
        }

        if (GUILayout.Button(new GUIContent("Figma Cleanup", "Figma cleanup many"), ToolbarStyles.buttonStyle)) {
            FigmaImporter.Editor.Tools.CleanupMany();
        }

        GUILayout.FlexibleSpace();

        // var tex = Resources.Load<Texture>("gem");
        // GUI.changed = false;
        // GUILayout.Toggle(m_enabled, new GUIContent(null, tex, "Focus SceneView when entering play mode"), "Command");
        // if (GUI.changed) {
        //     m_enabled = !m_enabled;
        // }
    }
}
#endif
