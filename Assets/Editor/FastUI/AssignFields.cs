using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AssignFields {
    public const string Namespace = "Axie";

    [MenuItem("Tools/FastUI/Assign Fields")]
    public static void Run() {
        var gameObjects = Selection.gameObjects;
        foreach (var gameObject in gameObjects) {
            var components = gameObject.GetComponents<MonoBehaviour>();
            if (Assign(components)) continue;
            // if assign current fail
            // then assign parents
            components = gameObject.GetComponentsInParent<MonoBehaviour>();
            Assign(components);
        }
    }

    private static bool Assign(IEnumerable<MonoBehaviour> components) {
        var has = false;
        foreach (var component in components) {
            var spaceName = component.GetType().Namespace;
            if (spaceName == null || !spaceName.Contains(Namespace)) continue;
            var val = Assign(component);
            has = has || val;
        }

        return has;
    }

    private static bool Assign(Component mono) {
        var has = false;
        var type = mono.GetType();
        const BindingFlags flags = BindingFlags.Public |
                                   BindingFlags.NonPublic |
                                   BindingFlags.Instance;

        var fields = type.GetFields(flags);

        // fields
        var componentType = typeof(Component);
        var gameObjectType = typeof(GameObject);
        foreach (var field in fields) {
            if (field.FieldType.IsSubclassOf(componentType)) {
                var value = field.GetValue(mono);
                if (value != null) {
                    var obj = value as Object;
                    if (obj != null && !string.IsNullOrEmpty(obj.name)) continue;
                }

                var val = AssignComponent(mono, field);
                has = has || val;
            } else if (field.FieldType == gameObjectType) {
                var value = field.GetValue(mono);
                if (value != null) {
                    var obj = value as Object;
                    if (obj != null && !string.IsNullOrEmpty(obj.name)) continue;
                }

                var val = AssignGameObject(mono, field);
                has = has || val;
            }
        }

        return has;
    }

    private static bool AssignComponent(Component target, FieldInfo field) {
        var keyword = $"@{field.Name.ToLower()}";
        var childs = target.GetComponentsInChildren(field.FieldType, true);
        var found = childs.FirstOrDefault(x => x.name.ToLower() == keyword);
        if (found == null) return false;
        field.SetValue(target, found);
        Debug.Log($"Assign {target.name}.<color=green>{field.Name}</color> : {field.FieldType}");
        return true;
    }

    private static bool AssignGameObject(Component target, FieldInfo field) {
        var keyword = $"@{field.Name.SnakeCase().ToTitleCase()}".Replace("_", string.Empty);
        var found = target.transform.FindChildByRecursion(keyword);
        if (found == null) return false;
        field.SetValue(target, found.gameObject);
        Debug.Log($"Assign {target.name}.<color=green>{field.Name}</color> : {field.FieldType}");
        return true;
    }
}
