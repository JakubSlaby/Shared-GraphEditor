using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.GraphEditor.View
{
	public class StackNodeView : UnityEditor.Experimental.GraphView.StackNode, INodeView
	{
		private IGraphNodeData m_Data;
		public IGraphNodeData data => m_Data;
		
		private List<Tuple<IGraphPortData, PortView>> m_InputPorts;
		private List<Tuple<IGraphPortData, PortView>> m_OutputPorts;
		
		Node INodeView.Node => this;
		
		public StackNodeView(IGraphNodeData nodeData)
		{
			m_Data = nodeData;
			userData = nodeData;
			headerContainer.Add(new Label(nodeData.GetType().Name));

			foreach (var inputPort in nodeData.InputPorts)
			{
				var port = new PortView(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, null);
				if(nodeData.InputPorts.Count <= 1)
					port.Q<Label>()?.RemoveFromHierarchy();
				this.inputContainer.Add(port);

				if (m_InputPorts == null)
					m_InputPorts = new List<Tuple<IGraphPortData, PortView>>();
				m_InputPorts.Add(new Tuple<IGraphPortData, PortView>(inputPort, port));
			}
			foreach (var outputPort in nodeData.OutputPorts)
			{
				var port = new PortView(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, null);
				if (nodeData.OutputPorts.Count <= 1)
					port.Q<Label>()?.RemoveFromHierarchy();
				else
					port.portName = outputPort.Id;
				this.outputContainer.Add(port);

				if (m_OutputPorts == null)
					m_OutputPorts = new List<Tuple<IGraphPortData, PortView>>();
				m_OutputPorts.Add(new Tuple<IGraphPortData, PortView>(outputPort, port));
			}

			var placeholder = this.Q("stackPlaceholderContainer", (string) null);
			placeholder.RemoveFromHierarchy();
			RefreshExpandedState();
			RefreshPorts();
		}
		
		
		public PortView GetPort(IGraphPortData portData)
		{
			var list = portData.Direction == GraphPortDirection.Input ? m_InputPorts : m_OutputPorts;
			if (list == null || list.Count == 0)
				return null;

			foreach (var tuple in list)
			{
				if (tuple.Item1 == portData)
					return tuple.Item2;
			}

			return null;
		}

	}
}