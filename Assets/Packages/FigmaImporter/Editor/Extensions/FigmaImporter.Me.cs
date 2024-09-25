using System;
using System.Collections.Generic;
using System.Linq;

namespace FigmaImporter.Editor {
    public partial class FigmaImporter {
        private void OnEnable() {
            FigmaImporterSettings.GetInstance();
            FontLinks.GetInstance();
            GradientsGenerator.GetInstance();
        }

        private static void PreprocessNode(Node node) {
            // remove invisible backgrounds
            if (node.background != null) {
                node.background.RemoveAll(x => !x.visible);
            }

            // remove invisible fills
            if (node.fills != null) {
                node.fills.RemoveAll(x => !x.visible);
            }

            // 9 slice
            if (Is9Slice(node)) {
                var fill = node.children[0].fills.FirstOrDefault(x => x.imageRef != null);
                node.fills = new List<Fill> { fill };
                node.is9Slice = true;
                node.children.Clear();
            }

            // set parent node
            if (node.children != null) {
                if (removeInvisible) {
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
    }
}
