using System.IO;
using UnityEditor;

namespace FigmaImporter.Editor {
    public partial class FontLinks {
        private static FontLinks _instance;

        public static FontLinks GetInstance() {
            if (_instance != null) return _instance;
            var assets = AssetDatabase.FindAssets("t:FontLinks");
            if (assets == null || assets.Length == 0) {
                _instance = CreateInstance<FontLinks>();
                if (!Directory.Exists("Assets/FigmaImporter/Editor")) {
                    Directory.CreateDirectory("Assets/FigmaImporter/Editor");
                }

                AssetDatabase.CreateAsset(_instance, "Assets/FigmaImporter/Editor/FontLinks.asset");
                AssetDatabase.Refresh();
                assets = AssetDatabase.FindAssets("t:FontLinks");
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
            _instance = AssetDatabase.LoadAssetAtPath<FontLinks>(assetPath);

            return _instance;
        }
    }
}
