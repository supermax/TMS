#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMS.Common.Extensions;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace TMS.Common.Core
{
	/// <summary>
	/// Delegate Command Inspector
	/// </summary>
	[CustomEditor(typeof(DelegateCommand))]
	public class DelegateCommandInspector : Editor
	{
		private readonly Dictionary<MonoBehaviour, IEnumerable<MethodInfo>> _methods =
		   new Dictionary<MonoBehaviour, IEnumerable<MethodInfo>>();

		private GameObject _prevTarget;
		private int _selectedComponentIndex = -1;
		private int _selectedMethodIndex = -1;

		/// <summary>
		/// Gets or sets the target.
		/// </summary>
		/// <value>
		/// The target.
		/// </value>
		protected virtual DelegateCommand Target { get; set; }

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		private void Init()
		{
			if (Target != null) return;

			Target = (DelegateCommand)target;
		}

		/// <summary>
		/// Initializes the method list.
		/// </summary>
		private void InitMethodList()
		{
			if (Target == null || Equals(_prevTarget, Target.CommandTarget)) return;

			_prevTarget = Target.CommandTarget;
			_methods.Clear();

			var scripts = Target.CommandTarget.GetComponents<MonoBehaviour>();
			if (scripts.IsNullOrEmpty()) return;

			foreach (var component in scripts)
			{
				var type = component.GetType();
				var methods = type.GetMethods();
				if (methods.IsNullOrEmpty()) continue;

				var methodsList = (from method in methods
								   where !method.IsConstructor &&
										 !method.IsAbstract &&
										 method.ReturnType == typeof(void)
								   let parameters = method.GetParameters()
								   where parameters.Length <= 1
								   select method);

				if (methodsList.IsNullOrEmpty()) continue;

				_methods.Add(component, methodsList);
				if (Target.CommandComponent == null || !Equals(Target.CommandComponent, component)) continue;

				_selectedComponentIndex = _methods.Keys.GetIndexOf(component);
				if (Target.CommandMethod.IsNullOrEmpty()) continue;

				var selectedMethod = methodsList.FirstOrDefault(m =>
					string.Equals(m.ToString(), Target.CommandMethod, StringComparison.InvariantCultureIgnoreCase));
				if (selectedMethod == null) continue;

				_selectedMethodIndex = methodsList.GetIndexOf(selectedMethod);
			}
		}

		/// <summary>
		/// Fired before the inspector GUI draw.
		/// </summary>
		protected virtual void BeforeInspectorGUI()
		{

		}

		/// <summary>
		/// Called when [inspector GUI].
		/// </summary>
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			Init();
			InitMethodList();

			BeforeInspectorGUI();

			GUILayout.BeginHorizontal();
			GUILayout.Label("IsEnabled");
			Target.IsEnabled = EditorGUILayout.Toggle(Target.IsEnabled);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Target");
			Target.CommandTarget = (GameObject)EditorGUILayout.ObjectField(Target.CommandTarget, typeof(GameObject));
			if (_methods.IsNullOrEmpty())
			{
				GUILayout.EndHorizontal();
				return;
			}

			var components = _methods.Keys.Select(component => component.GetType().Name).ToArray();
			GUILayout.Label("Component");
			_selectedComponentIndex = EditorGUILayout.Popup(_selectedComponentIndex, components);
			if (_selectedComponentIndex < 0 || _selectedComponentIndex >= _methods.Keys.Count)
			{
				GUILayout.EndHorizontal();
				return;
			}

			var selectedComponent = _methods.Keys.ElementAt(_selectedComponentIndex);
			if (Target != null)
			{
				Target.CommandComponent = selectedComponent;
			}

			var methods = _methods[selectedComponent];
			if (methods.IsNullOrEmpty())
			{
				GUILayout.EndHorizontal();
				return;
			}

			var methodNames = methods.Select(method =>
			{
				var parameters = method.GetParameters();
				var parameter = parameters.IsNullOrEmpty() ? null : parameters[0];
				var methodName = string.Format("{0}({1})", method.Name, parameter != null ? parameter.Name : null);
				return methodName;
			}).ToArray();
			GUILayout.EndHorizontal();

			GUILayout.Label("Method", GUILayout.Width(50));
			_selectedMethodIndex = EditorGUILayout.Popup(_selectedMethodIndex, methodNames);
			if (_selectedMethodIndex < 0 || _selectedMethodIndex >= methods.Count()) return;

			Target.CommandMethod = methods.ElementAt(_selectedMethodIndex).ToString();

			GUILayout.BeginHorizontal();
			if (Target.CommandParameter2.IsNullOrEmpty())
			{
				GUILayout.Label("Param1", GUILayout.Width(50));
				Target.CommandParameter1 = EditorGUILayout.ObjectField(Target.CommandParameter1, typeof(Object), GUILayout.Width(120));
			}

			if (Target.CommandParameter1 == null)
			{
				GUILayout.Space(25);
				GUILayout.Label("Param2", GUILayout.Width(50));
				Target.CommandParameter2 = GUILayout.TextField(Target.CommandParameter2, GUILayout.Width(120));
			}
			GUILayout.EndHorizontal();
		}
	}
}
#endif