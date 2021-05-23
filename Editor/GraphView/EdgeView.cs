using UnityEditor.Experimental.GraphView;
using WhiteSparrow.Shared.GraphEditor.Data;

namespace WhiteSparrow.Shared.GraphEditor.View
{
	public class EdgeView : Edge
	{
		private IGraphEdgeData m_Data;
		public IGraphEdgeData data => m_Data;

		public EdgeView()
		{
		}
		public EdgeView(IGraphEdgeData data)
		{
			m_Data = data;
			capabilities = Capabilities.Selectable;
		}


		
	}

}