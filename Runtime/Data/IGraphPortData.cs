#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

namespace WhiteSparrow.Shared.GraphEditor.Data
{
	public interface IGraphPortData
	{
		string Id { get; }
		IGraphNodeData Node { get; }
		GraphPortDirection Direction { get; }
	}

	public enum GraphPortDirection
	{
		Input = 0,
		Output = 1
	}

	public static class GraphPortDataExtensions
	{
#if UNITY_EDITOR
		public static Direction ToUnityGraphDirection(this GraphPortDirection direction)
		{
			return direction == GraphPortDirection.Input ? Direction.Input : Direction.Output;
		}

		public static GraphPortDirection ToGraphDirection(this Direction direction)
		{
			return direction == Direction.Input ? GraphPortDirection.Input : GraphPortDirection.Output;
		}
#endif
	}
}