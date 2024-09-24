using System.Linq;
using UnityEditor;

public class FastUIHelper {
    [MenuItem("Tools/FastUI/Enable")]
    public static void Enable() {
        AddDefineSymbolForGroup("FAST_UI", BuildTargetGroup.iOS);
        AddDefineSymbolForGroup("FAST_UI", BuildTargetGroup.Android);
        AddDefineSymbolForGroup("FAST_UI", BuildTargetGroup.Standalone);
    }

    [MenuItem("Tools/FastUI/Disable")]
    public static void Disable() {
        RemoveDefineSymbolForGroup("FAST_UI", BuildTargetGroup.iOS);
        RemoveDefineSymbolForGroup("FAST_UI", BuildTargetGroup.Android);
        RemoveDefineSymbolForGroup("FAST_UI", BuildTargetGroup.Standalone);
    }

    public static void AddDefineSymbolForGroup(string defineSymbol, BuildTargetGroup group) {
        var defineSymbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        var defineSymbols = defineSymbolsString.Split(';').ToList();

        if (defineSymbols.Contains(defineSymbol)) return;
        defineSymbols.Add(defineSymbol);
        var newDefineSymbolsString = string.Join(";", defineSymbols.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(@group, newDefineSymbolsString);
    }

    public static void RemoveDefineSymbolForGroup(string defineSymbol, BuildTargetGroup group) {
        var defineSymbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        var defineSymbols = defineSymbolsString.Split(';').ToList();

        if (!defineSymbols.Contains(defineSymbol)) return;
        defineSymbols.Remove(defineSymbol);
        var newDefineSymbolsString = string.Join(";", defineSymbols.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(@group, newDefineSymbolsString);
    }
}
