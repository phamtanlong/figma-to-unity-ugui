﻿using System;
using System.Collections.Generic;
using FigmaImporter.Editor.EditorTree.TreeData;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FigmaImporter.Editor
{
    public partial class FigmaImporterSettings : ScriptableObject
    {
        [SerializeField] private string clientCode;
        [SerializeField] private string state;
        [SerializeField] private string token;
        [SerializeField] private string url;
        [SerializeField] private string rendersPath = "FigmaImporter/Renders";
        [SerializeField] public string PresetsPath = "FigmaImporter/Presets";
        [SerializeField] public string tempPath = "Temp/Figma";
        [SerializeField] public bool showLoading = true;
        [SerializeField] public bool quickButton;
        [SerializeField] public bool showTree = true;
        [SerializeField] public bool lazyPreview = true;
        [SerializeField] public int delay = 100;
        [SerializeField] public bool removeInvisible = true;
        [Header("Texts")]
        [SerializeField] public bool textAutoSize;
        [SerializeField] public bool textRaycastTarget;
        [Header("Hard code Action for Object by Name")]
        [SerializeField] public List<StringAction> nameActions = new List<StringAction>();
        [Header("Hard code Render for Components")]
        [SerializeField] public List<ComponentInfo> renderComponents = new List<ComponentInfo>();

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

    [Serializable]
    public class StringAction {
        public string Name;
        public ActionType Action;
    }

    [Serializable]
    public class ComponentInfo {
        public string Id;
        public string Name;
    }
}
