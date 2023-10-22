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

		public void SetGraph(IGraphData graph)
		{
			ClearGraph();
			m_Graph = graph;

			if (m_Graph == null)
				return;
			BuildGraph();
			schedule.Execute(LayoutGraph);
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
		internal static string GetScriptPath()
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


#region Layout
		
		private void LayoutGraph()
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

			schedule.Execute(() => FrameAll());
		}


#endregion

#region Flow

		
		private List<IGraphNodeData> m_HelperActiveNodes = new List<IGraphNodeData>();
		private List<IGraphEdgeData> m_HelperActiveEdges = new List<IGraphEdgeData>();
		private void UpdateFlowState()
		{
			var flowGraph = m_Graph as IFlowGraphData;
			if (flowGraph == null)
				return;
			
			m_HelperActiveNodes.Clear();
			m_HelperActiveEdges.Clear();
			
			var flows = flowGraph.Flows;
			foreach (var flowData in flows)
			{
				m_HelperActiveNodes.AddRange(flowData.ActiveNodes);
				m_HelperActiveEdges.AddRange(flowData.ActiveEdges);
			}

			nodes.ForEach(node =>
			{
				INodeView nodeView = node as INodeView;
				IFlowNodeView flowView = node as IFlowNodeView;
				if (nodeView == null || flowView == null)
					return;

				var data = nodeView.data;
				var active = m_HelperActiveNodes.Contains(data);
				if (active)
				{
					if(data.TimeEnded > data.TimeActivated)
						flowView.SetFlowState(FlowNodeState.Complete);
					else
						flowView.SetFlowState(FlowNodeState.Active);
					
					return;
				}

				float maxLimit = 0.4f;
				float strength = Time.realtimeSinceStartup - data.TimeEnded;
				if (strength >= maxLimit)
				{
					flowView.SetFlowState(FlowNodeState.Inactive);
					return;
				}
				
				flowView.SetFlowState(FlowNodeState.Complete, 1 - (strength / maxLimit));
			});

			edges.ForEach(edge =>
			{
				EdgeView edgeView = edge as EdgeView;
				IGraphEdgeData edgeData = edgeView?.data;
				if (edgeData == null)
					return;

				bool active = m_HelperActiveEdges.Contains(edgeData);
				if (active)
				{
					edge.input.portColor = Color.yellow;
					edge.output.portColor = Color.yellow;
				}
				else
				{
					edge.input.portColor = new Color(0.9411765f, 0.9411765f, 0.9411765f);
					edge.output.portColor = new Color(0.9411765f, 0.9411765f, 0.9411765f);
				}
			});
			
			m_HelperActiveNodes.Clear();
		}

#endregion

		public void Update()
		{
			UpdateFlowState();
		}
	}
}