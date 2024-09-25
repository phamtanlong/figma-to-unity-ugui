using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace FigmaImporter.Editor
{
    [CreateAssetMenu(menuName = "FigmaImporter/FontLinks")]
    public partial class FontLinks : ScriptableObject
    {
        [SerializeField] public List<FontStringPair> _fonts = new List<FontStringPair>();

        public TMP_FontAsset Get(string fontName)
        {
            var font = _fonts.FirstOrDefault(x => x.Name == fontName);
            return font?.Font;
        }

        public void AddName(string fontName)
        {
            if (_fonts.FirstOrDefault(x => x.Name == fontName) == null)
                _fonts.Add(new FontStringPair(fontName, null));
        }
    }

    [Serializable]
    public class FontStringPair
    {
        public string Name;
        public TMP_FontAsset Font;

        public FontStringPair(string name, TMP_FontAsset font)
        {
            Name = name;
            Font = font;
        }
    }
}
