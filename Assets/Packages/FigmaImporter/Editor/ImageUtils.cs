using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
#if VECTOR_GRAHICS_IMPORTED
using Unity.VectorGraphics;
#endif

namespace FigmaImporter.Editor
{
    public class ImageUtils
    {
        public static void AddOverridenSprite(GameObject nodeGo, Sprite overridenSprite)
        {
            var image = nodeGo.AddComponent<Image>();
            image.sprite = overridenSprite;
        }

#if VECTOR_GRAHICS_IMPORTED
        public static void AddOverridenSvgSprite(GameObject nodeGo, Sprite overridenSprite)
        {
            var image = nodeGo.AddComponent<SVGImage>();
            image.sprite = overridenSprite;
        }

        public static async Task RenderSvgNodeAndApply(Node node, GameObject nodeGo, FigmaImporter importer)
        {
            FigmaNodesProgressInfo.CurrentInfo = "Loading image";
            FigmaNodesProgressInfo.ShowProgress(0f);
            var result = await importer.GetSvgImage(node.id);
            string svgAsString = result == null? null : Encoding.UTF8.GetString(result);
            if (svgAsString == null || svgAsString.Contains("image/jpg") || svgAsString.Contains("image/jpeg") || svgAsString.Contains("image/png"))
            {
                Debug.LogError("It seems that svg contains raster image. It is not supported by Unity Vector Graphics. Trying to load raster image instead.");
                await RenderNodeAndApply(node, nodeGo, importer);
                return;
            }
            string spriteName = $"{node.name}_{node.id.Replace(':', '_')}.svg";
            var destinationPath = $"/{importer.GetRendersFolderPath()}/{spriteName}";
            try
            {
                SaveSvgTexture(result, destinationPath);
                using (var stream = new StreamReader(Application.dataPath + destinationPath))
                    SVGParser.ImportSVG(stream, ViewportOptions.DontPreserve, 0, 1, 100, 100);
                var t = nodeGo.transform as RectTransform;
            }
            catch (Exception e)
            {
                Debug.LogError("It seems that svg cant be imported. Trying to load raster image instead." + e.Message);
                if (File.Exists(destinationPath))
                    File.Delete(destinationPath);
                await RenderNodeAndApply(node, nodeGo, importer);
                return;
            }

            SVGImage image = null;
            Sprite sprite = null;
            FigmaNodesProgressInfo.CurrentInfo = "Saving rendered node";
            FigmaNodesProgressInfo.ShowProgress(0f);
            try
            {
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets{destinationPath}");
                image = nodeGo.AddComponent<SVGImage>();
                image.sprite = sprite;
                image.preserveAspect = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private static void SaveSvgTexture(byte[] bytes, string path)
        {
             var filePath = Application.dataPath + path;
             System.IO.File.WriteAllBytes(filePath, bytes);
             UnityEditor.AssetDatabase.Refresh();

        }

#endif

        public static Sprite ChangeTextureToSprite(string path)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter == null) return null;
            textureImporter.textureType = TextureImporterType.Sprite;
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path);
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        // private static void SaveTexture(Texture2D texture, string path)
        // {
        //     var filePath = Path.Combine(Application.dataPath, path);
        //     if (File.Exists(filePath)) return;
        //     byte[] bytes = texture.EncodeToPNG();
        //     if (bytes != null)
        //     {
        //         File.WriteAllBytes(filePath, bytes);
        //         AssetDatabase.Refresh();
        //     }
        // }

        public static void SaveTexture(Texture2D texture, Node node, FigmaImporter importer) {
            try {
                if (importer == null)
                    importer = (FigmaImporter) EditorWindow.GetWindow(typeof(FigmaImporter));
                var spriteName = node.spriteName();// $"{node.name}_{node.id.Replace(':', '_')}.png";
                var path = Path.Combine(importer.GetRendersFolderPath(), spriteName);// $"{importer.GetRendersFolderPath()}/{spriteName}";
                var filePath = Path.Combine(Application.dataPath, path);
                if (File.Exists(filePath)) return;
                var bytes = texture.EncodeToPNG();
                if (bytes == null) return;
                File.WriteAllBytes(filePath, bytes);
                AssetDatabase.Refresh();
                Debug.Log($"Save Image: {Path.GetFileName(path)}");
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public static void SetMask(Node node, GameObject nodeGo)
        {
            if (!node.clipsContent)
                return;
            if (node.fills.Count == 0)
                nodeGo.AddComponent<RectMask2D>();
            else
                nodeGo.AddComponent<Mask>();
        }

        public static async Task RenderNodeAndApply(Node node, GameObject nodeGo, FigmaImporter importer)
        {
            FigmaNodesProgressInfo.CurrentInfo = "Loading image";
            FigmaNodesProgressInfo.ShowProgress(0f);

            string spriteName = node.spriteName();// $"{node.name}_{node.id.Replace(':', '_')}.png";
            var path = Path.Combine(importer.GetRendersFolderPath(), spriteName);// $"{importer.GetRendersFolderPath()}/{spriteName}";
            var absolutePath = Path.Combine(Application.dataPath, path);
            var assetPath = Path.Combine("Assets", path);

            Texture2D result;
            if (File.Exists(absolutePath)) {
                result = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            } else {
                result = await importer.GetImage(node, node.id, true, 0);
            }

            if (result == null) {
                Debug.LogError($"Can not render node: {node.name}_{node.id.Replace(':', '_')}");
                return;
            }

            var t = nodeGo.transform as RectTransform;
            Image image = null;
            Sprite sprite = null;
            FigmaNodesProgressInfo.CurrentInfo = "Saving rendered node";
            FigmaNodesProgressInfo.ShowProgress(0f);
            try
            {
                SaveTexture(result, node, importer);
                sprite = ImageUtils.ChangeTextureToSprite(assetPath);//$"Assets/{importer.GetRendersFolderPath()}/{spriteName}");
                if (sprite == null) {
                    Debug.LogError($"Null sprite: {spriteName}");
                    return;
                }

                image = nodeGo.AddComponent<Image>();
                image.sprite = sprite;
                return; // I disable check size below

                // // magic here, I don't know why
                // if (Math.Abs(t.rect.width - sprite.texture.width) < 2f &&
                //     Math.Abs(t.rect.height - sprite.texture.height) < 2f)
                // {
                //     image = nodeGo.AddComponent<Image>();
                //     image.sprite = sprite;
                //     return;
                // }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            var child = TransformUtils.InstantiateChild(nodeGo, "Render");
            if (sprite != null)
            {
                image = child.AddComponent<Image>();
                image.sprite = sprite;
                t = child.transform as RectTransform;
                t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sprite.texture.width);
                t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sprite.texture.height);
            }
        }
    }
}
