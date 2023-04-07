using System.Collections.Generic;

namespace WhiteSparrow.Shared.GraphEditor.Data
{
	/// <summary>
	/// Interface defining a graph compatible with the Graph Editor
	/// </summary>
	public interface IGraphData
	{
		/// <summary>
		/// Collection of all nodes in the graph.
		/// </summary>
		IReadOnlyCollection<IGraphNodeData> Nodes { get; }
		/// <summary>
		/// Collection of all edges (connections) between node ports.
		/// </summary>
		IReadOnlyCollection<IGraphEdgeData> Edges { get; }
		
	}

	public interface IAutoLayoutGraphData
	{
#if UNITY_EDITOR
		Microsoft.Msagl.Core.Layout.GeometryGraph ToMSAL();
#endif
	}

	public interface IFlowGraphData
	{
		/// <summary>
		/// Collection of active logical flows currently running.
		/// </summary>
		IReadOnlyCollection<IGraphFlowData> Flows { get; }
	}
}