using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace WhiteSparrow.Shared.GraphEditor.View
{
	public class SplitView : VisualElement
	{
		public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits>
		{
		}

		private VisualElement m_Content;
		public readonly VisualElement Left;
		public readonly VisualElement Right;
		private VisualElement m_Anchor;

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
				
			string path = new System.Diagnostics.StackTrace(0, true).GetFrame(0).GetFileName();
			if (string.IsNullOrEmpty(path))
				return null;

			int indexOfAssets = path.LastIndexOf("Assets", StringComparison.Ordinal);
			if (indexOfAssets >= 0)
				path = path.Substring(indexOfAssets);
			path = path.Replace("SplitView.cs", "SplitView.uss");
		
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

			FetchStyleSheet();
			m_Content = new VisualElement();
			m_Content.name = "unity-content-container";
			m_Content.AddToClassList("unity-two-pane-split-view--horizontal");
			m_Content.AddToClassList("unity-two-pane-split-view_content-container");
			
			m_Anchor = new VisualElement();
			m_Anchor.name = "unity-dragline-anchor";
			m_Anchor.AddToClassList("unity-two-pane-split-view__dragline-anchor");
			m_Anchor.AddToClassList("unity-two-pane-split-view__dragline-anchor--horizontal");

			Left = new VisualElement();
			Left.name = "left-column";
			Right = new VisualElement();
			Right.name = "right-column";
			
			m_Content.Add(Left);
			m_Content.Add(m_Anchor);
			m_Content.Add(Right);
			hierarchy.Add(m_Content);
			
			m_Anchor.AddManipulator(new SplitViewDragManipulator(Left));
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