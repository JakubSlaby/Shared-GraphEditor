using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.GraphEditor.View
{
	public class PortView : Port
	{
		public PortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
		{
			m_EdgeConnector = new EdgeConnector<EdgeView>(new DefaultEdgeConnectorListener());
			this.AddManipulator(m_EdgeConnector);
		}

		private class DefaultEdgeConnectorListener : IEdgeConnectorListener
		{
			public DefaultEdgeConnectorListener()
			{
			}
			
			public void OnDropOutsidePort(Edge edge, Vector2 position)
			{
				CustomGraphView graphView = edge.GetFirstAncestorOfType<CustomGraphView>();
				EdgeView obsoleteEdge = edge as EdgeView;
				IGraphEdgeData edgeData = obsoleteEdge?.data;
				if (edgeData == null)
					return;

				EdgeView newEdge = new EdgeView(edgeData);
				newEdge.input = graphView.GetPort(edgeData.input);
				newEdge.output = graphView.GetPort(edgeData.output);
				newEdge.input.Connect(newEdge);
				newEdge.output.Connect(newEdge);
				graphView.AddElement(newEdge);
				
			}

			public void OnDrop(GraphView gv, Edge e)
			{
				CustomGraphView graphView = gv as CustomGraphView;
				EdgeView edge = e as EdgeView;
				if (edge == null || edge.data == null)
					return;
				
				edge.input = graphView.GetPort(edge.data.input);
				edge.output = graphView.GetPort(edge.data.output);
				edge.input.Connect(edge);
				edge.output.Connect(edge);
				graphView.AddElement(edge);
			}
		}

	}
}