using System.IO;
using UnityEditor;

namespace FigmaImporter.Editor
{
    public partial class FigmaImporterSettings
    {
        private static FigmaImporterSettings _settings;

        public static FigmaImporterSettings GetInstance()
        {
            if (_settings != null) return _settings;
            var assets = AssetDatabase.FindAssets("t:FigmaImporterSettings");
            if (assets == null || assets.Length == 0) {
                _settings = CreateInstance<FigmaImporterSettings>();
                if (!Directory.Exists("Assets/FigmaImporter/Editor")) {
                    Directory.CreateDirectory("Assets/FigmaImporter/Editor");
                }

                AssetDatabase.CreateAsset(_settings, "Assets/FigmaImporter/Editor/FigmaImporterSettings.asset");
                AssetDatabase.Refresh();
                assets = AssetDatabase.FindAssets("t:FigmaImporterSettings");
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
            _settings = AssetDatabase.LoadAssetAtPath<FigmaImporterSettings>(assetPath);

            return _settings;
        }
    }
}
