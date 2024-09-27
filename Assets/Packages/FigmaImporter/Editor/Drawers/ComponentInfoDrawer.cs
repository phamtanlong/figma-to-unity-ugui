using System;
using FigmaImporter.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ComponentInfo))]
public class ComponentInfoDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var IdField = new PropertyField(property.FindPropertyRelative("Id"), "Id");
        var NameField = new PropertyField(property.FindPropertyRelative("Name"));

        // Add fields to the container.
        container.Add(IdField);
        container.Add(NameField);

        return container;
    }

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var idRect = new Rect(position.x, position.y, position.width / 2 - 1, position.height);
        var nameRect = new Rect(position.x + position.width / 2 + 1, position.y, position.width / 2, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(idRect, property.FindPropertyRelative("Id"), GUIContent.none);
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("Name"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
