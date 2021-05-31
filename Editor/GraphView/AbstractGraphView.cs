using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Miscellaneous;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteSparrow.Shared.GraphEditor.Data;
using Port = UnityEditor.Experimental.GraphView.Port;

namespace WhiteSparrow.Shared.GraphEditor.View
{
	public class AbstractGraphView : GraphView
	{
		private IGraphData m_Graph;
		private AbstractGraphViewManipulators m_DefaultManipulators;

		public AbstractGraphView()
		{
			this.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Path.Combine(GetScriptPath(), "GraphView.uss")));
		
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
			
			m_DefaultManipulators = new AbstractGraphViewManipulators(this);

			var grid = new GridBackground();
			Insert(0, grid);
			
		}

		private void OnLayout()
		{
			var autoLayoutGraphData = m_Graph as IAutoLayoutGraphData;
			if (autoLayoutGraphData == null)
				return;

			GeometryGraph geometryGraph = autoLayoutGraphData.ToMSAL();
			SugiyamaLayoutSettings layoutSettings = new SugiyamaLayoutSettings();
			layoutSettings.Transformation = PlaneTransformation.Rotation(Math.PI);
			LayoutHelpers.CalculateLayout(geometryGraph, layoutSettings, null);
			geometryGraph.UpdateBoundingBox();

			var nodes = geometryGraph.Nodes;
			foreach (var node in nodes)
			{
				var nodeData = node.UserData as IGraphNodeData;
				if (nodeData == null)
					continue;
				Rect position = new Rect(new Vector2((float)node.Center.X, (float)node.Center.Y),
									new Vector2((float)node.Width, (float)node.Height));
				position.position -= position.size * 0.5f;
				var nodeView = GetNode(nodeData);
				nodeView.Node.SetPosition(position);
			}

			this.schedule.Execute(() => FrameAll());
		}

		public void SetGraph(IGraphData graph)
		{
			ClearGraph();
			m_Graph = graph;

			if (m_Graph == null)
				return;
			BuildGraph();
			schedule.Execute(OnLayout);
			schedule.Execute(UpdateFlowState);
		}

		private void BuildGraph()
		{
			var nodeDataList = m_Graph.Nodes;
			foreach (var nodeData in nodeDataList)
			{
				var node = new StackNodeView(nodeData);
				node.SetPosition(nodeData.position);
				this.AddElement(node);
			}

			var edgeDataList = m_Graph.Edges;
			foreach (var edgeData in edgeDataList)
			{
				var edge = new EdgeView(edgeData);
				edge.input = GetPort(edgeData.input);
				edge.output = GetPort(edgeData.output);
				edge.input.Connect(edge);
				edge.output.Connect(edge);
				this.AddElement(edge);
			}
		}

		private void ClearGraph()
		{
			DeleteElements(nodes.ToList());
			DeleteElements(edges.ToList());
		}

		private List<IGraphNodeData> m_HelperActiveNodes = new List<IGraphNodeData>();
		private void UpdateFlowState()
		{
			var flows = m_Graph.Flows;
			foreach (var flowData in flows)
			{
				m_HelperActiveNodes.AddRange(flowData.ActiveNodes);
			}

			nodes.ForEach(node =>
			{
				INodeView nodeView = node as INodeView;
				IFlowNodeView flowView = node as IFlowNodeView;
				if (nodeView == null || flowView == null)
					return;
				var active = m_HelperActiveNodes.Contains(nodeView.data);
				flowView.SetFlowState(active ? FlowNodeState.Active : FlowNodeState.Inactive);
			});
			
			m_HelperActiveNodes.Clear();

			if (this.parent != null)
				schedule.Execute(UpdateFlowState);
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			List<Port> output = new List<Port>();
			base.ports.ForEach(port =>
			{
				if (startPort != port && startPort.node != port.node && startPort.direction != port.direction)
					output.Add(port);
			});
			return output;
		}

		public INodeView GetNode(IGraphNodeData data)
		{
			return nodes.ToList().FirstOrDefault(n => n is INodeView nv && nv.data == data) as INodeView;
		}

		public PortView GetPort(IGraphPortData data)
		{
			return GetNode(data.Node)?.GetPort(data);
		}

		public EdgeView GetEdge(IGraphEdgeData data)
		{
			return edges.ToList().FirstOrDefault(e => e is EdgeView ev && ev.data == data) as EdgeView;
		}
		
		
		
		
		private static string s_CachedScriptPath;
		private static string GetScriptPath()
		{
			if (!string.IsNullOrEmpty(s_CachedScriptPath))
			{
				return s_CachedScriptPath;
			}
				
			string path = new System.Diagnostics.StackTrace(0, true).GetFrame(0).GetFileName();
			if (string.IsNullOrEmpty(path))
				return null;

			int indexOfAssets = path.LastIndexOf("Assets", StringComparison.Ordinal);
			if (indexOfAssets >= 0)
				path = path.Substring(indexOfAssets);
			path = path.Replace("AbstractGraphView.cs", string.Empty);
			
			return s_CachedScriptPath = path;
		}
		
		
	}
}