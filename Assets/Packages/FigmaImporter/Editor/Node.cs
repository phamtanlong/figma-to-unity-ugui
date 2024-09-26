using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace FigmaImporter.Editor
{
    [Serializable]
    public partial class Node
    {
        public string id;
        public string name;
        public bool visible = true; // default true, do not change
        public string type;
        public Dictionary<string, NodeProperty> componentProperties;
        public string blendMode;
        public List<Node> children;
        public AbsoluteBoundingBox absoluteBoundingBox; // done
        public Constraints constraints; // done
        public bool clipsContent;
        public List<Fill> background;
        public List<Fill> fills;
        public Fill[] strokes;
        public float strokeWeight;
        public string strokeAlign;
        public Color backgroundColor;
        public Grid[] layoutGrids;
        public Effect[] effects;
        public string characters;
        public Style style;
        public string transitionNodeID;
        public float transitionDuration;
        public string transitionEasing;
        [JsonIgnore] public Node parent;

        public string spriteName() {
            if (is9Slice) return $"{name}.png";

            var layer = getInstanceLayer(this, 0);
            var layers = id.Replace(':', '_').Split(';').ToList();

            var ids = new List<string>();
            for (var i = 0; i < layer && i <= layers.Count - 1; i++) {
                ids.Add(layers[layers.Count - 1 - i]);
            }

            ids.Reverse();

            return ids.Count > 0 ? $"{name}_{string.Join(";", ids)}.png" : $"{name}_{string.Join(";", layers)}.png";
        }

        public string objectName() {
            if (type == "INSTANCE" && componentProperties != null) {
                var (_, value) = componentProperties.FirstOrDefault(x => x.Value.type == "VARIANT");
                if (value?.type == "VARIANT") {
                    return $"{name}~{value.value}";
                }
            }

            if (type == "COMPONENT" && name.Contains('=')) {
                if (parent?.type == "COMPONENT_SET") {
                    return parent.name + "~" + name.Split('=')[1];
                }
            }

            return name;
        }

        private static int getInstanceLayer(Node node, int count) {
            while (true) {
                if (node.type == "INSTANCE") return count;
                if (node.parent == null) return 0;
                node = node.parent;
                count += 1;
            }
        }
    }

    [Serializable]
    public class AbsoluteBoundingBox
    {
        public float x;
        public float y;
        public float width;
        public float height;

        public Vector2 GetPosition()
        {
            return new Vector2(x, y);
        }

        public Vector2 GetSize()
        {
            return new Vector2(width, height);
        }
    }

    [Serializable]
    public class Constraints
    {
        public string vertical;
        public string horizontal;
    }
    [Serializable]
    public class Fill
    {
        public string blendMode;
        public bool visible = true;
        public string type;
        public Color color;
        public string imageRef;
        public Vector[] gradientHandlePositions;
        public GradientStops[] gradientStops;
    }
    [Serializable]
    public class Color
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public UnityEngine.Color ToColor()
        {
            return new UnityEngine.Color(r,g,b,a);
        }
    }

    [Serializable]
    public class Grid
    {
        public string pattern;
        public float sectionSize;
        public bool visible = true;
        public Color color;
        public string alignment;
        public float gutterSize;
        public float offset;
        public int count;
    }

    [Serializable]
    public class Effect
    {
        public string type;
        public bool visible = true;
        public Color color;
        public string blendMode;
        public Vector offset;
        public float radius;
    }

    [Serializable]
    public class Vector
    {
        public float x;
        public float y;

        public Vector2 ToVector2()
        {
            return new Vector2(x,y);
        }
    }

    [Serializable]
    public class GradientStops
    {
        public Color color;
        public float position;
    }

    [Serializable]
    public class Style
    {
        public string fontFamily;
        public string fontPostScriptName;
        public int fontWeight;
        public float fontSize;
        public string textAlignHorizontal;
        public string textAlignVertical;
        public float letterSpacing;
        public float lineHeightPx;
        public float lineHeightPercent;
        public string lineHeightUnit;
        public string textCase;
        public string textDecoration;
    }

    [Serializable]
    public enum FontWeight
    {
        Thin = 100, Light = 300, Regular = 400, Medium = 500, Bold = 700, Black = 900,
        ThinItalic = 100
    }

    [Serializable]
    public class NodeProperty {
        public string value;
        public string type;
    }
}
