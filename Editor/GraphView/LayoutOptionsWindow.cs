using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Layout.Incremental;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Prototype.Ranking;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;

namespace WhiteSparrow.Shared.GraphEditor.View
{
	public class LayoutOptionsWindow : EditorWindow
	{
		public static void ShowWindow(AbstractGraphView graphView)
		{
			LayoutOptionsWindow window = EditorWindow.GetWindow<LayoutOptionsWindow>();
			window.m_TargetGraphView = graphView;
			window.Show();
		}

		private AbstractGraphView m_TargetGraphView;

		private void OnEnable()
		{
			Construct();
		}

		private Type[] m_LayoutTypes = new Type[]
		{
			typeof(FastIncrementalLayoutSettings),
			typeof(RankingLayoutSettings),
			typeof(SugiyamaLayoutSettings),
		};


		private PopupField<Type> settingsTypeSelector;
		private VisualElement fieldsContainer;

		private Type selectedType;
		private LayoutAlgorithmSettings settings;

		
		private void Construct()
		{
			var selfRoot = new VisualElement();
			settingsTypeSelector = new PopupField<Type>(m_LayoutTypes.ToList(), 0);
			settingsTypeSelector.value = m_LayoutTypes[0];
			settingsTypeSelector.RegisterCallback<ChangeEvent<Type>>(OnSettingsTypeChange);

			fieldsContainer = new VisualElement();
			
			selfRoot.Add(settingsTypeSelector);
			selfRoot.Add(fieldsContainer);

			this.rootVisualElement.Add(selfRoot);
		}

		private Dictionary<VisualElement, PropertyInfo> fieldToPropertyMapping =
			new Dictionary<VisualElement, PropertyInfo>();
		private void OnSettingsTypeChange(ChangeEvent<Type> evt)
		{
			if (selectedType == evt.newValue)
				return;

			fieldsContainer.Clear();

			selectedType = evt.newValue;
			settings = Activator.CreateInstance(evt.newValue) as LayoutAlgorithmSettings;
			if (settings == null)
				return;

			var fields = selectedType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (var field in fields)
			{
				object value = null;
				try
				{
					value = field.GetValue(settings);
				}
				catch (Exception e)
				{
					continue;
				}
				var type = field.PropertyType;
				
				BindableElement item = null;
				if (type == typeof(int))
				{
					var intfield = new IntegerField(field.Name);
					intfield.value = (int) value;
					intfield.RegisterValueChangedCallback(OnChangeInt);
					item = intfield;
				}
				else if (type == typeof(double))
				{
					var dbfield = new DoubleField(field.Name);
					dbfield.value = (double) value;
					dbfield.RegisterValueChangedCallback(OnChangeDouble);
					item = dbfield;
				}
				else if (type == typeof(float))
				{
					var flfield = new FloatField(field.Name);
					flfield.value = (float) value;
					flfield.RegisterValueChangedCallback(OnChangeFloat);

					item = flfield;
				}
				else if (type == typeof(bool))
				{
					var blfield = new Toggle(field.Name);
					blfield.value = (bool) value;
					blfield.RegisterValueChangedCallback(OnChangeBool);

					item = blfield;
				}

				if (item == null)
					continue;

				fieldToPropertyMapping[item] = field;
				
				fieldsContainer.Add(item);
			}
		}

		private void OnChangeBool(ChangeEvent<bool> evt)
		{
			ProcessChange(evt.target as VisualElement, evt.newValue);
		}

		private void OnChangeFloat(ChangeEvent<float> evt)
		{
			ProcessChange(evt.target as VisualElement, evt.newValue);
		}

		private void OnChangeDouble(ChangeEvent<double> evt)
		{
			ProcessChange(evt.target as VisualElement, evt.newValue);
		}

		private void OnChangeInt(ChangeEvent<int> evt)
		{
			ProcessChange(evt.target as VisualElement, evt.newValue);
		}

		private void ProcessChange(VisualElement ve, object value)
		{
			if (!fieldToPropertyMapping.TryGetValue(ve, out PropertyInfo prop))
				return;

			try
			{
				prop.SetValue(settings, value);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return;
			}
			if(settings != null)
				m_TargetGraphView.SetLayout(settings);
		}
	}
}