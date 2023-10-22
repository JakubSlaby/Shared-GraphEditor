using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.GraphEditor.View
{
	public interface INodeView
	{
		IGraphNodeData data { get; }
		PortView GetPort(IGraphPortData portData);
		Node Node { get; }
	}
	
	public class NodeView : Node, INodeView, IFlowNodeView
	{
		private IGraphNodeData m_Data;
		public IGraphNodeData data => m_Data;

		private List<Tuple<IGraphPortData, PortView>> m_InputPorts;
		private List<Tuple<IGraphPortData, PortView>> m_OutputPorts;
		
		Node INodeView.Node => this;
		
		private NodeViewAdditionalElements m_NodeElements;

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

			var flowOutline = new VisualElement();
			flowOutline.name = "graph-flow-outline";
			hierarchy.Insert(0, flowOutline);

			m_NodeElements = new NodeViewAdditionalElements(this);
			
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

		void IFlowNodeView.SetFlowState(FlowNodeState state)
		{
			m_NodeElements.SetFlowState(state);
		}

		public void SetFlowState(FlowNodeState state, float strength)
		{
			m_NodeElements.SetFlowState(state, strength);
		}

		FlowNodeState IFlowNodeView.FlowState => m_NodeElements.FlowState;
	}

	internal class NodeViewAdditionalElements : IFlowNodeView
	{
		private VisualElement m_Target;
		private VisualElement m_FlowIndicator;

		private FlowNodeState m_FlowState;
		
		public NodeViewAdditionalElements(VisualElement target)
		{
			m_Target = target;

			m_FlowIndicator = new VisualElement();
			m_FlowIndicator.name = "graph-flow-outline";
			m_FlowIndicator.AddToClassList("graph-flow-outline");
			m_Target.hierarchy.Insert(0, m_FlowIndicator);

		}

		public void SetFlowState(FlowNodeState state)
		{
			m_FlowState = state;
			m_FlowIndicator.RemoveFromClassList("graph-flow-outline--inactive");
			m_FlowIndicator.RemoveFromClassList("graph-flow-outline--active");
			m_FlowIndicator.RemoveFromClassList("graph-flow-outline--complete");
			m_FlowIndicator.style.opacity = new StyleFloat(100);
			switch (state)
			{
				case FlowNodeState.Active:
					m_FlowIndicator.AddToClassList("graph-flow-outline--active");
					break;
				case FlowNodeState.Complete:
					m_FlowIndicator.AddToClassList("graph-flow-outline--complete");
					break;
				case FlowNodeState.Inactive:
					m_FlowIndicator.AddToClassList("graph-flow-outline--inactive");
					break;
			}
		}

		public void SetFlowState(FlowNodeState state, float strength)
		{
			SetFlowState(state);
			m_FlowIndicator.style.opacity = new StyleFloat(strength);
		}

		public FlowNodeState FlowState => m_FlowState;
	}
}