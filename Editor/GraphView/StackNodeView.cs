using System;
using System.Collections.Generic;
using Plugins.Repositories.GraphEditor.Runtime.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.GraphEditor.View
{
	public class StackNodeView : StackNode, INodeView, IFlowNodeView
	{
		private IGraphNodeData m_Data;
		public IGraphNodeData data => m_Data;
		
		private List<Tuple<IGraphPortData, PortView>> m_InputPorts;
		private List<Tuple<IGraphPortData, PortView>> m_OutputPorts;
		
		Node INodeView.Node => this;

		private NodeViewAdditionalElements m_NodeElements;

		public StackNodeView(IGraphNodeData nodeData)
		{
			m_Data = nodeData;
			userData = nodeData;
			headerContainer.Add(new Label(nodeData.NodeDisplayName));

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

			if (nodeData is INestedGraphNodeData nestedNodeData)
			{
				var nestedView = new Button();
				nestedView.text = nestedNodeData.NestedGraphDisplayName;
				nestedView.clicked += () => Debug.Log("CLICK");
				mainContainer.Add(nestedView);
			}

			var placeholderContainer = this.Q("stackPlaceholderContainer", (string) null);
			placeholderContainer.RemoveFromHierarchy();
			
			var separatorContainer = this.Q("stackSeparatorContainer", (string) null);
			separatorContainer.RemoveFromHierarchy();

			m_NodeElements = new NodeViewAdditionalElements(this);
			
			RefreshExpandedState();
			RefreshPorts();
		}
		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			base.BuildContextualMenu(evt);
			
			if(data is IGraphDataSource)
				evt.menu.AppendAction("Open Script", ContextualOpenScript);
		}

		private void ContextualOpenScript(DropdownMenuAction obj)
		{
			if (data is IGraphDataSource dataSource)
			{
				GraphDataSource.OpenScriptOfType(data.GetType());
			}
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
}