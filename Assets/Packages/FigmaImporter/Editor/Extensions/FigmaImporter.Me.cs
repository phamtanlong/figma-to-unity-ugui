using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FigmaImporter.Editor {
    public partial class FigmaImporter {
        private void OnEnable() {
            FigmaImporterSettings.GetInstance();
            FontLinks.GetInstance();
            GradientsGenerator.GetInstance();
        }

        private void PreprocessNode(Node node) {
            FigmaTemp.GetInstance().totalNode++;
            if (node.type == "COMPONENT") {
                FigmaTemp.GetInstance().components.Add(new ComponentInfo {
                    Id = node.id, Name = node.name
                });
            }

            // remove invisible backgrounds
            if (node.background != null) {
                node.background.RemoveAll(x => !x.visible);
            }

            // remove invisible fills
            if (node.fills != null) {
                node.fills.RemoveAll(x => !x.visible);
            }

            // 9 slice
            node.is9Slice = Is9Slice(node);
            // if (Is9Slice(node)) {
            //     var fill = node.children[0].fills.FirstOrDefault(x => x.imageRef != null);
            //     node.fills = new List<Fill> { fill };
            //     node.is9Slice = true;
            //     node.children.Clear();
            // }

            // set parent node
            if (node.children != null) {
                if (FigmaImporterSettings.GetInstance().removeInvisible) {
                    node.children.RemoveAll(x => !x.visible);
                }

                foreach (var child in node.children) {
                    child.parent = node;
                    PreprocessNode(child);
                }
            }
        }

        public static bool Is9Slice(Node node) {
            if (node.children == null || node.children.Count < 2) return false;

            for (var i = 0; i < node.children.Count - 1; i++) {
                if (node.children[i].fills == null || node.children[i].fills.Count == 0) return false;
                if (node.children[i + 1].fills == null || node.children[i + 1].fills.Count == 0) return false;
                if (node.children[i].fills[0].imageRef == null) return false;
                if (node.children[i].fills[0].imageRef != node.children[i + 1].fills[0].imageRef) return false;
            }

            return true;
        }

        public static Texture2D LoadTempTexture(string fileName) {
            var tempPath = FigmaImporterSettings.GetInstance().tempPath;
            if (!Directory.Exists(tempPath)) {
                Directory.CreateDirectory(tempPath);
            }

            var path = Path.Combine(tempPath, fileName);
            if (!File.Exists(path)) {
                return null;
            }

            var imageData = File.ReadAllBytes(path);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            // Optionally set the texture filter mode and wrap mode
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            return texture;
        }

        public static bool SaveTempTexture(string fileName, Texture2D texture) {
            var tempPath = FigmaImporterSettings.GetInstance().tempPath;
            if (!Directory.Exists(tempPath)) {
                Directory.CreateDirectory(tempPath);
            }

            var path = Path.Combine(tempPath, fileName);
            var bytes = texture.EncodeToPNG();
            if (bytes == null) return false;
            File.WriteAllBytes(path, bytes);
            return true;
        }
    }
}