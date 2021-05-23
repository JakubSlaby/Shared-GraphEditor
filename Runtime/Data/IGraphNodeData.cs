using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteSparrow.Shared.GraphEditor.Data
{
	public interface IGraphNodeData
	{
		Guid guid { get; }
		Rect position { get; set; }
		
		IReadOnlyCollection<IGraphPortData> InputPorts { get; }
		IReadOnlyCollection<IGraphPortData> OutputPorts { get; }
	}
}