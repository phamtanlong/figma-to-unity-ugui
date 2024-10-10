using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FigmaImporter.Editor {
    [CreateAssetMenu(menuName = "FigmaImporter/FigmaTemp")]
    public partial class FigmaTemp : ScriptableObject {
        [SerializeField] public int totalNode;
        [SerializeField] public List<ComponentInfo> components = new();

        private static FigmaTemp _instance;

        public static FigmaTemp GetInstance() {
            if (_instance != null) return _instance;
            var assets = AssetDatabase.FindAssets("t:FigmaTemp");
            if (assets == null || assets.Length == 0) {
                _instance = CreateInstance<FigmaTemp>();
                if (!Directory.Exists("Assets/FigmaImporter/Editor")) {
                    Directory.CreateDirectory("Assets/FigmaImporter/Editor");
                }

                AssetDatabase.CreateAsset(_instance, "Assets/FigmaImporter/Editor/FigmaTemp.asset");
                AssetDatabase.Refresh();
                assets = AssetDatabase.FindAssets("t:FigmaTemp");
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
            _instance = AssetDatabase.LoadAssetAtPath<FigmaTemp>(assetPath);

            return _instance;
        }
    }
}
