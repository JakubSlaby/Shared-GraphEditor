using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace WhiteSparrow.Shared.GraphEditor.Elements
{
	public class SplitView : VisualElement
	{
		public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits>
		{
		}

		public readonly VisualElement Left;
		public readonly VisualElement Right;

		public override VisualElement contentContainer => Left;

		private static string s_StyleSheetPath;
		private static StyleSheet FetchStyleSheet()
		{
			if (!string.IsNullOrEmpty(s_StyleSheetPath))
			{
				var s1 = AssetDatabase.LoadAssetAtPath<StyleSheet>(s_StyleSheetPath);
				if (s1 != null)
					return s1;
				s_StyleSheetPath = null;
			}

			string path =  GraphEditorUtil.FindAssetPathToCallingScript("SplitView.uss");
			if (string.IsNullOrEmpty(path))
				return null;
		
			var s2 = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
			if (s2 == null)
				return null;
			
			s_StyleSheetPath = path;
			return s2;
		}
		
		public SplitView()
		{
			styleSheets.Add(FetchStyleSheet());
			this.AddToClassList("unity-two-pane-split-view");
			this.AddToClassList("unity-two-pane-split-view--horizontal");
			this.AddToClassList("unity-two-pane-split-view_content-container");

			var anchor = new VisualElement();
			anchor.name = "unity-dragline-anchor";
			anchor.AddToClassList("unity-two-pane-split-view__dragline-anchor");
			anchor.AddToClassList("unity-two-pane-split-view__dragline-anchor--horizontal");

			var anchorExtender = new VisualElement();
			anchorExtender.AddToClassList("anchor-extender");
			anchor.hierarchy.Add(anchorExtender);

			Left = new VisualElement();
			Left.name = "left-column";
			Right = new VisualElement();
			Right.name = "right-column";
			
			hierarchy.Add(Left);
			hierarchy.Add(anchor);
			hierarchy.Add(Right);
			
			anchorExtender.AddManipulator(new SplitViewDragManipulator(Left));
		}
		
		public class SplitViewDragManipulator : MouseManipulator
		{
			private bool m_Active;
			private VisualElement m_ResizeTarget;
			private StyleLength m_Length;

			public SplitViewDragManipulator(VisualElement resizeTarget)
			{
				m_ResizeTarget = resizeTarget;
			}
			
			protected override void RegisterCallbacksOnTarget()
			{
				target.RegisterCallback(new EventCallback<MouseDownEvent>(OnMouseDown), TrickleDown.NoTrickleDown);
				target.RegisterCallback(new EventCallback<MouseMoveEvent>(OnMouseMove), TrickleDown.NoTrickleDown);
				target.RegisterCallback(new EventCallback<MouseUpEvent>(OnMouseUp), TrickleDown.NoTrickleDown);
			}

			protected override void UnregisterCallbacksFromTarget()
			{
				target.UnregisterCallback(new EventCallback<MouseDownEvent>(OnMouseDown), TrickleDown.NoTrickleDown);
				target.UnregisterCallback(new EventCallback<MouseMoveEvent>(OnMouseMove), TrickleDown.NoTrickleDown);
				target.UnregisterCallback(new EventCallback<MouseUpEvent>(OnMouseUp), TrickleDown.NoTrickleDown);

			}
			private void OnMouseDown(MouseDownEvent evt)
			{
				target.CaptureMouse();
				evt.StopPropagation();
				m_Active = true;
				m_Length = m_ResizeTarget.layout.width;
			}
			private void OnMouseMove(MouseMoveEvent evt)
			{
				if (!m_Active)
					return;
				m_Length = Mathf.Round(m_Length.value.value + evt.mouseDelta.x);
				m_ResizeTarget.style.width = m_Length;
				
			}
			private void OnMouseUp(MouseUpEvent evt)
			{
				m_Active = false;
				if(target.HasMouseCapture())
					target.ReleaseMouse();
				evt.StopPropagation();
			}
		}
	}
}