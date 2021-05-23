using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.GraphEditor.View
{
	public class NodeView : Node
	{
		private IGraphNodeData m_Data;
		public IGraphNodeData data => m_Data;

		private List<Tuple<IGraphPortData, PortView>> m_InputPorts;
		private List<Tuple<IGraphPortData, PortView>> m_OutputPorts;
		
		public NodeView(IGraphNodeData nodeData)
		{
			m_Data = nodeData;
			userData = nodeData;
			title = nodeData.GetType().Name;

			foreach (var inputPort in nodeData.InputPorts)
			{
				var port = new PortView(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, null);
				port.portName = inputPort.Id;
				this.inputContainer.Add(port);

				if (m_InputPorts == null)
					m_InputPorts = new List<Tuple<IGraphPortData, PortView>>();
				m_InputPorts.Add(new Tuple<IGraphPortData, PortView>(inputPort, port));
			}
			foreach (var outputPort in nodeData.OutputPorts)
			{
				var port = new PortView(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, null);
				port.portName = outputPort.Id;
				this.outputContainer.Add(port);

				if (m_OutputPorts == null)
					m_OutputPorts = new List<Tuple<IGraphPortData, PortView>>();
				m_OutputPorts.Add(new Tuple<IGraphPortData, PortView>(outputPort, port));
			}

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