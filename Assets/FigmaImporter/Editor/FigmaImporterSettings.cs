using UnityEditor;
using UnityEngine;

namespace FigmaImporter.Editor
{
    public class FigmaImporterSettings : ScriptableObject
    {
        [SerializeField] private string clientCode = null;
        [SerializeField] private string state = null;
        [SerializeField] private string token = null;
        [SerializeField] private string url = null;
        [SerializeField] private string rendersPath = "FigmaImporter/Renders";
        [SerializeField] public string PresetsPath = "FigmaImporter/Presets";

        public string ClientCode
        {
            get => clientCode;
            set => clientCode = value;
        }

        public string State
        {
            get => state;
            set => state = value;
        }

        public string Token
        {
            get => token;
            set => token = value;
        }

        public string Url
        {
            get => url;
            set => url = value;
        }

        public string RendersPath
        {
            get => rendersPath;
            set => rendersPath = value;
        }

        private static FigmaImporterSettings _settings;

        public static FigmaImporterSettings GetInstance()
        {
            if (_settings != null) return _settings;
            var assets = AssetDatabase.FindAssets("t:FigmaImporterSettings");
            if (assets == null || assets.Length == 0)
            {
                _settings = CreateInstance<FigmaImporterSettings>();
                AssetDatabase.CreateAsset(_settings, "Assets/FigmaImporter/Editor/FigmaImporterSettings.asset");
                AssetDatabase.Refresh();
            }
            else
            {

                var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
                _settings = AssetDatabase.LoadAssetAtPath<FigmaImporterSettings>(assetPath);
            }

            return _settings;
        }
    }
}
