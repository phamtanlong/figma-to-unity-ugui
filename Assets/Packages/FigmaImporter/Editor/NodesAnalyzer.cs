using System.Collections.Generic;
using System.Linq;
using FigmaImporter.Editor.EditorTree.TreeData;

namespace FigmaImporter.Editor
{
    public class NodesAnalyzer
    {
        public static void AnalyzeRenderMode(IList<Node> nodes, IList<NodeTreeElement> nodesTreeElements)
        {
            foreach (var node in nodes)
            {
                AnalyzeSingleNode(node, nodesTreeElements.FirstOrDefault(x=>x.figmaId == node.id));
                if (node.children != null)
                {
                    AnalyzeRenderMode(node.children, nodesTreeElements);
                }
            }
        }

        public static void AnalyzeTransformMode(IList<Node> nodes, IList<NodeTreeElement> nodesTreeElements)
        {
            foreach (var node in nodes)
            {
                nodesTreeElements.FirstOrDefault(x => x.figmaId == node.id).actionType = ActionType.Transform;
                if (node.children != null)
                {
                    AnalyzeTransformMode(node.children, nodesTreeElements);
                }
            }
        }

        public static void AnalyzeSVGMode(IList<Node> nodes, IList<NodeTreeElement> nodesTreeElements)
        {
            foreach (var node in nodes)
            {
                AnalyzeSingleNode(node, nodesTreeElements.FirstOrDefault(x=>x.figmaId == node.id));
                if (node.children != null)
                {
                    AnalyzeSVGMode(node.children, nodesTreeElements);
                }
            }
        }

        public static ActionType AnalyzeSingleNode(Node node) {
            var render =
#if VECTOR_GRAHICS_IMPORTED
            ActionType.SvgRender;
#else
            ActionType.Render;
#endif

            var setting = FigmaImporterSettings.GetInstance();

            // preset list
            var nameAction = setting.nameActions.FirstOrDefault(x => x.Name == node.name);
            if (nameAction != null) return nameAction.Action;

            // compoents in list
            var componentInfo = setting.renderComponents.FirstOrDefault(x => x.Id == node.componentId);
            if (componentInfo != null) return ActionType.Render;

            if (node.is9Slice) return ActionType.Render;

            if (node.type != "TEXT" && (node.children == null || node.children.Count == 0)) {
                return render;
            }

            return ActionType.Generate;
        }

        private static void AnalyzeSingleNode(Node node, NodeTreeElement treeElement)
        {
            treeElement.actionType = AnalyzeSingleNode(node);
        }

        public static void CheckActions(IList<Node> nodes, IList<NodeTreeElement> nodesTreeElements)
        {
            if (nodes == null)
                return;

            foreach (var node in nodes)
            {
                var treeElement = nodesTreeElements.First(x=>x.figmaId == node.id);
                if (treeElement.actionType == ActionType.Render)
                {
                    SetChildrenActionRecursively(node.children, ActionType.None, nodesTreeElements);
                }
                else
                {
                    CheckActions(node.children, nodesTreeElements);
                }
            }
        }

        private static void SetChildrenActionRecursively(IList<Node> nodes, ActionType actionType,
            IList<NodeTreeElement> nodesTreeElements)
        {
            if (nodes == null)
                return;
            foreach (var node in nodes)
            {
                nodesTreeElements.First(x => x.figmaId == node.id).actionType = actionType;
                SetChildrenActionRecursively(node.children, actionType, nodesTreeElements);
            }
        }
    }
}
