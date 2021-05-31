namespace WhiteSparrow.Shared.GraphEditor.View
{
	public enum FlowNodeState
	{
		Inactive = 0,
		Active = 1,
		Complete = 2
	}
	
	public interface IFlowNodeView
	{
		void SetFlowState(FlowNodeState state);
		FlowNodeState FlowState { get; }
	}
}