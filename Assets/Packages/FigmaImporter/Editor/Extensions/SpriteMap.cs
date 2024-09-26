using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FigmaImporter.Editor {
    [CreateAssetMenu(menuName = "FigmaImporter/TextureMap")]
    public partial class TextureMap : ScriptableObject {
        public Dictionary<string, Texture2D> map = new Dictionary<string, Texture2D>();

        public void Add(string id, Texture2D texture2D) {
            if (map.ContainsKey(id)) return;
            map.Add(id, texture2D);
        }

        public Texture2D Find(string id) {
            return map.ContainsKey(id) ? map[id] : null;
        }

        private static TextureMap _instance;

        public static TextureMap GetInstance() {
            if (_instance != null) return _instance;
            var assets = AssetDatabase.FindAssets("t:TextureMap");
            if (assets == null || assets.Length == 0) {
                _instance = CreateInstance<TextureMap>();
                if (!Directory.Exists("Assets/FigmaImporter/Editor")) {
                    Directory.CreateDirectory("Assets/FigmaImporter/Editor");
                }

                AssetDatabase.CreateAsset(_instance, "Assets/FigmaImporter/Editor/TextureMap.asset");
                AssetDatabase.Refresh();
                assets = AssetDatabase.FindAssets("t:TextureMap");
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
            _instance = AssetDatabase.LoadAssetAtPath<TextureMap>(assetPath);

            return _instance;
        }
    }
}
