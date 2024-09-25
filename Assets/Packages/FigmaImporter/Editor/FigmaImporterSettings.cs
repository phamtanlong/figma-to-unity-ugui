using UnityEngine;

namespace FigmaImporter.Editor
{
    public partial class FigmaImporterSettings : ScriptableObject
    {
        [SerializeField] private string clientCode = null;
        [SerializeField] private string state = null;
        [SerializeField] private string token = null;
        [SerializeField] private string url = null;
        [SerializeField] private string rendersPath = "FigmaImporter/Renders";
        [SerializeField] public string PresetsPath = "FigmaImporter/Presets";
        [Header("Texts")]
        [SerializeField] public bool TextAutoSize = false;
        [SerializeField] public bool TextRaycastTarget = false;

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
    }
}
