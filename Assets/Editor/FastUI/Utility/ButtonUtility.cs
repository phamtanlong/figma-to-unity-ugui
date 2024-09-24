using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class ButtonUtility {
    public const BindingFlags flags = BindingFlags.Public |
                                      BindingFlags.NonPublic |
                                      BindingFlags.Static |
                                      BindingFlags.Instance;

    public const BindingFlags publicFlags = BindingFlags.Public | BindingFlags.Instance;

    public static void AddButtonListener(Button button, Object target) {
        if (button == null) return;

        var buttonName = button.name.SnakeCase().ToLower()
            .Replace("@", string.Empty)
            .Replace("-", string.Empty)
            .Replace(" ", string.Empty)
            .Replace("btn", string.Empty)
            .Replace("button", string.Empty)
            .Trim('-', '_', ' ');

        var voidType = typeof(void);
        var targetType = target.GetType();

        var methods = target.GetType().GetMethods(publicFlags);
        foreach (var method in methods) {
            if (method.GetParameters().Length > 0) continue;
            if (method.ReturnType != voidType) continue;
            // if (method.DeclaringType != targetType) continue;

            var methodName = method.Name.SnakeCase().ToLower()
                .Replace("on_", string.Empty)
                .Replace("btn", string.Empty)
                .Replace("button", string.Empty)
                .Replace("clicked", string.Empty)
                .Replace("click", string.Empty)
                .Trim('-', '_', ' ');

            if (!methodName.Contains(buttonName)) continue;

            AddPersistentListener(button.onClick, target, method, UnityEventCallState.RuntimeOnly);
            return;
        }
    }

    public static void RemovePersistentListener(UnityEvent unityEvent, Object target, MethodInfo methodInfo) {
        var methods = unityEvent.GetType().GetMethods(flags);
        foreach (var method in methods) {
            if (method.Name != "RemovePersistentListener") continue;
            if (method.GetParameters().Length != 0) continue;
            method.Invoke(unityEvent, null);
            // Debug.Log("<color=green>RemovePersistentListener</color>");
            return;
        }

        Debug.LogError("RemovePersistentListener");
    }

    public static void AddPersistentListener(UnityEvent unityEvent, Object target, MethodInfo methodInfo,
        UnityEventCallState callState) {
        var persistentEventCount = unityEvent.GetPersistentEventCount();
        R_AddPersistentListener(unityEvent);
        RegisterPersistentListener(unityEvent, persistentEventCount, target, methodInfo);
        unityEvent.SetPersistentListenerState(persistentEventCount, callState);
        Debug.Log($"Component <color=green>{target.name}</color> listen event: {methodInfo.Name}");
    }

    public static void AddPersistentListener(UnityEvent unityEvent, UnityAction call, UnityEventCallState callState) {
        var persistentEventCount = unityEvent.GetPersistentEventCount();
        R_AddPersistentListener(unityEvent);
        RegisterPersistentListener(unityEvent, persistentEventCount, call);
        unityEvent.SetPersistentListenerState(persistentEventCount, callState);
    }

    public static void RegisterPersistentListener(UnityEvent unityEvent, int index, Object target,
        MethodInfo methodInfo) {
        if (methodInfo == null)
            Debug.LogWarning((object)"Registering a Listener requires an action");
        else {
            R_RegisterPersistentListener(unityEvent, index, target, methodInfo.DeclaringType, methodInfo);
        }
    }

    public static void RegisterPersistentListener(UnityEvent unityEvent, int index, UnityAction call) {
        if (call == null)
            Debug.LogWarning((object)"Registering a Listener requires an action");
        else
            R_RegisterPersistentListener(unityEvent, index, call.Target, call.Method.DeclaringType, call.Method);
    }

    // ---- reflection methods --------

    private static void R_AddPersistentListener(UnityEvent unityEvent) {
        // reflection to call this method `internal void UnityEvent.AddPersistentListener()`

        var methods = unityEvent.GetType().GetMethods(flags);
        foreach (var method in methods) {
            if (method.Name != "AddPersistentListener") continue;
            if (method.GetParameters().Length != 0) continue;
            method.Invoke(unityEvent, null);
            // Debug.Log("<color=green>AddPersistentListener</color>");
            return;
        }

        Debug.LogError("AddPersistentListener");
    }

    private static void R_RegisterPersistentListener(UnityEvent unityEvent, int index, object targetObj,
        System.Type targetObjType, MethodInfo methodInfo) {
        // reflection to call this method `internal void UnityEvent.RegisterPersistentListener(int, object, Type, MethodInfo)`

        var methods = unityEvent.GetType().GetMethods(flags);
        foreach (var method in methods) {
            if (method.Name != "RegisterPersistentListener") continue;
            if (method.GetParameters().Length != 4) continue;
            method.Invoke(unityEvent, new[] { index, targetObj, targetObjType, methodInfo });
            // Debug.Log("<color=green>RegisterPersistentListener</color>");
            return;
        }

        Debug.LogError("RegisterPersistentListener");
    }

    private enum SnakeCaseState {
        Start,
        Lower,
        Upper,
        NewWord
    }

    public static string SnakeCase(this string s) {
        if (string.IsNullOrEmpty(s))
            return s;
        var stringBuilder = new StringBuilder();
        var snakeCaseState = SnakeCaseState.Start;
        for (var index = 0; index < s.Length; ++index)
            if (s[index] == ' ') {
                if (snakeCaseState != SnakeCaseState.Start)
                    snakeCaseState = SnakeCaseState.NewWord;
            } else if (char.IsUpper(s[index]) || char.IsNumber(s[index])) {
                switch (snakeCaseState) {
                    case SnakeCaseState.Lower:
                    case SnakeCaseState.NewWord:
                        stringBuilder.Append('_');
                        break;
                    case SnakeCaseState.Upper:
                        var flag = index + 1 < s.Length;
                        if ((index > 0) & flag) {
                            var c = s[index + 1];
                            if (!char.IsUpper(c) && c != '_') stringBuilder.Append('_');
                        }

                        break;
                }

                var lower = char.ToLower(s[index], CultureInfo.InvariantCulture);
                stringBuilder.Append(lower);
                snakeCaseState = SnakeCaseState.Upper;
            } else if (s[index] == '_') {
                stringBuilder.Append('_');
                snakeCaseState = SnakeCaseState.Start;
            } else {
                if (snakeCaseState == SnakeCaseState.NewWord)
                    stringBuilder.Append('_');
                stringBuilder.Append(s[index]);
                snakeCaseState = SnakeCaseState.Lower;
            }

        return stringBuilder.ToString();
    }

    public static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
    public static string ToTitleCase(this string input) {
        return textInfo.ToTitleCase(input.ToLower());
    }


    public static Transform FindChildByRecursion(this Transform aParent, string aName) {
        if (aParent == null) return null;
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent) {
            result = child.FindChildByRecursion(aName);
            if (result != null)
                return result;
        }

        return null;
    }
}
