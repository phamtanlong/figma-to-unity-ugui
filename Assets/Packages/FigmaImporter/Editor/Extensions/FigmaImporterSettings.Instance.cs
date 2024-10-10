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

        public void IncreaseDelay() {
            delay = (int)(delay * 1.5f);
            if (delay > 5000) delay = 5000;
        }

        public void DecreaseDelay() {
            if (delay > 220) {
                delay -= 20;
            }
        }

        public void ResetDelay() {
            delay = 100;
        }
    }
}
