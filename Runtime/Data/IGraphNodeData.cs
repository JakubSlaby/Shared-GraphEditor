using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteSparrow.Shared.GraphEditor.Data
{
	public interface IGraphNodeData
	{
		Guid guid { get; }
		Rect position { get; set; }
		
		float TimeActivated { get; }
		float TimeEnded { get; }
		
		IReadOnlyCollection<IGraphPortData> InputPorts { get; }
		IReadOnlyCollection<IGraphPortData> OutputPorts { get; }
		
		string NodeDisplayName { get; }
	}

	public interface INestedGraphNodeData : IGraphNodeData
	{
		Type NestedGraphType { get; }
		IGraphData NestedGraph { get; }
		
		string NestedGraphDisplayName { get; }
	}
}