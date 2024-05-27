using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.GraphEditor.View
{
    public abstract class AbstractGraphViewContainer<T> : VisualElement, IGraphViewContainer
        where T : CustomGraphView, new()
    {
        private T m_GraphView;
        public T GraphView => m_GraphView;
        
        private IGraphData m_Graph;
        private List<IGraphData> m_Breadcrumbs = new List<IGraphData>();

        private Toolbar m_Toolbar;
        private ToolbarBreadcrumbs m_ToolbarBreadcrumbs;
        
        public AbstractGraphViewContainer()
        {
            this.AddToClassList("sparrowGraphEditor-container");
            this.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(GraphEditorUtil.FindAssetPathToCallingScript("GraphView.uss")));
            
            
            hierarchy.Add(m_Toolbar = new Toolbar());
            m_Toolbar.AddToClassList("sparrowGraphEditor-container__toolbar");
            
            m_Toolbar.Add(m_ToolbarBreadcrumbs = new ToolbarBreadcrumbs());
            
            
            m_GraphView = new T();
            hierarchy.Add(m_GraphView);
        }
        
        public virtual void ShowGraph(IGraphData graph)
        {
            Cleanup();
            ShowNestedGraph(graph);
        }

        public virtual void Cleanup()
        {
            var items = m_Breadcrumbs.ToArray();
            m_Breadcrumbs.Clear();

            foreach (var graph in items)
            {
                OnGraphRemoved(graph);
            }
            
            m_GraphView.SetGraph(null);
        }
        


        public virtual void ShowNestedGraph(INestedGraphNodeData nestedGraphNode)
        {
            ShowNestedGraph(nestedGraphNode.NestedGraph);
        }
        
        public virtual void ShowNestedGraph(IGraphData graph)
        {
            int index = m_Breadcrumbs.IndexOf(graph);
            if (m_Breadcrumbs.Count > 0 && index == m_Breadcrumbs.Count - 1)
                return;

            if (index != -1)
            {
                var removedGraphs = m_Breadcrumbs.GetRange(index + 1, m_Breadcrumbs.Count - (index + 1));
                m_Breadcrumbs.RemoveRange(index + 1, m_Breadcrumbs.Count - (index + 1));

                foreach (var removedGraph in removedGraphs)
                {
                    OnGraphRemoved(removedGraph);
                }
            }
            else
            {
                m_Breadcrumbs.Add(graph);
            }
            
            m_Graph = graph;
            m_GraphView.SetGraph(graph);

            OnGraphAdded(graph);
            
            RebuildBreadcrumbs();
        }

        private void RebuildBreadcrumbs()
        {
            m_ToolbarBreadcrumbs.Clear();
            foreach (var graph in m_Breadcrumbs)
            {
                m_ToolbarBreadcrumbs.PushItem(graph.GetType().Name, () => ShowNestedGraph(graph));
            }
        }
        
        protected virtual void OnGraphAdded(IGraphData graph)
        {
            
        }

        protected virtual void OnGraphRemoved(IGraphData graph)
        {
            
        }
    }

    public interface IGraphViewContainer
    {
        void ShowGraph(IGraphData graph);
        void ShowNestedGraph(IGraphData graph);
        void ShowNestedGraph(INestedGraphNodeData nestedGraphNode);
    }
}