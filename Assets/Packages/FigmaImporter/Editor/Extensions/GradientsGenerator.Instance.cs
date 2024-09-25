using System.IO;
using UnityEditor;

namespace FigmaImporter.Editor {
    public partial class GradientsGenerator {
        private static GradientsGenerator _instance;

        public static GradientsGenerator GetInstance() {
            if (_instance != null) return _instance;
            var assets = AssetDatabase.FindAssets("t:GradientsGenerator");
            if (assets == null || assets.Length == 0) {
                _instance = CreateInstance<GradientsGenerator>();
                if (!Directory.Exists("Assets/FigmaImporter/Editor")) {
                    Directory.CreateDirectory("Assets/FigmaImporter/Editor");
                }

                AssetDatabase.CreateAsset(_instance, "Assets/FigmaImporter/Editor/GradientsGenerator.asset");
                AssetDatabase.Refresh();
                assets = AssetDatabase.FindAssets("t:GradientsGenerator");
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
            _instance = AssetDatabase.LoadAssetAtPath<GradientsGenerator>(assetPath);

            return _instance;
        }
    }
}
