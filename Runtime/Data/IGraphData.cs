using System.Collections.Generic;
using Microsoft.Msagl.Core.Layout;

namespace WhiteSparrow.Shared.GraphEditor.Data
{
	public interface IGraphData
	{
		IReadOnlyCollection<IGraphNodeData> Nodes { get; }
		IReadOnlyCollection<IGraphEdgeData> Edges { get; }
		
		
	}

	public interface IAutoLayoutGraphData
	{
		GeometryGraph ToMSAL();
	}
}