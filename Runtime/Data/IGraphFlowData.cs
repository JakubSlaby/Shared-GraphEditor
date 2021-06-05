using System.Collections.Generic;

namespace WhiteSparrow.Shared.GraphEditor.Data
{
	public delegate void GraphFlowChangeDelegate(); 
	
	public interface IGraphFlowData
	{
		IReadOnlyCollection<IGraphNodeData> ActiveNodes { get; }
		IReadOnlyCollection<IGraphEdgeData> ActiveEdges { get; }
	}
}