namespace WhiteSparrow.Shared.GraphEditor.Data
{
	public interface IGraphEdgeData
	{
		IGraphPortData input { get; }
		IGraphPortData output { get; }
	}
}