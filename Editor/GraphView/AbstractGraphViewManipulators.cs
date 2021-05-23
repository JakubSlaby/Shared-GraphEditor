using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace WhiteSparrow.Shared.GraphEditor.View
{
	public class AbstractGraphViewManipulators
	{
		private AbstractGraphView m_GraphView;

		private ContentDragger m_ContentDragger;
		private SelectionDragger m_SelectionDragger;
		private RectangleSelector m_RectangleSelector;
		private ClickSelector m_ClickSelector;
		
		public AbstractGraphViewManipulators(AbstractGraphView graphView)
		{
			m_GraphView = graphView;
			
			m_GraphView.AddManipulator(m_ContentDragger = new ContentDragger());
			m_GraphView.AddManipulator(m_SelectionDragger = new SelectionDragger());
			m_GraphView.AddManipulator(m_RectangleSelector = new RectangleSelector());
			m_GraphView.AddManipulator(m_ClickSelector = new ClickSelector());
		}
	}
}