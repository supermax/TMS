#region Code Editor
// Maxim
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using TMS.Common.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TMS.Common.Extensions
{
	public static class MonoBehaviourExtensions
	{
		public static void Invoke(this MonoBehaviour sender, Task task, float time)
		{
			sender.Invoke(task.Method.Method.Name, time);
		}

		public static T GetComponent<T>(this Component sender) where T : class
		{
			return sender.GetComponent(typeof(T)) as T;
		}

		public static List<T> GetObjects<T>(this MonoBehaviour sender) where T : class
		{
			var monoBehaviours = Object.FindObjectsOfType<MonoBehaviour>();
			var res = monoBehaviours.Select(behaviour => behaviour.GetComponent(typeof(T))).OfType<T>().CreateList();
			return res;
		}

		public static T GetSafeComponent<T>(this Component sender) where T : MonoBehaviour
		{
			var component = sender.GetComponent<T>();
			if (component == null)
			{
				Debug.LogError(string.Format("Expected to find component of type '{0}' but found none.", typeof(T)), sender);
			}
			return component;
		}

		/// <summary>
		/// Add a new child game object.
		/// </summary>
		public static GameObject AddChild(this GameObject parent, bool undo = true)
		{
			var go = new GameObject();
#if UNITY_EDITOR
		if (undo) UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
			if (parent == null) return go;

			var t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			go.layer = parent.layer;
			return go;
		}

		/// <summary>
		/// Instantiate an object and add it to the specified parent.
		/// </summary>
		public static GameObject AddChild(this GameObject parent, GameObject prefab, bool resetTransform = false)
		{
			var go = Object.Instantiate(prefab);
#if UNITY_EDITOR
		UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
			if (go == null || parent == null) return go;

			var t = go.transform;
			//t.parent = parent.transform;
			t.SetParent(parent.transform);

			if (resetTransform)
			{
				t.localPosition = Vector3.zero;
				t.localRotation = Quaternion.identity;
				t.localScale = Vector3.one;
			}
			else
			{
				t.localPosition = prefab.transform.position;
				t.localRotation = prefab.transform.rotation;
				t.localScale = prefab.transform.localScale;
			}

			go.layer = parent.layer;
			return go;
		}

	    private static readonly Dictionary<System.Type, string> TypeNames = new Dictionary<Type, string>();

		/// <summary>
		/// Helper function that returns the string name of the type.
		/// </summary>
		public static string GetTypeName<T>()
		{
			var s = typeof(T).ToString();
			if (s.StartsWith("UI")) s = s.Substring(2);
			else if (s.StartsWith("UnityEngine.")) s = s.Substring(12);
			return s;
		}

		/// <summary>
		/// Helper function that returns the string name of the type.
		/// </summary>
		public static string GetTypeName(UnityEngine.Object obj)
		{
			if (obj == null) return "Null";
			var s = obj.GetType().ToString();
			if (s.StartsWith("UI")) s = s.Substring(2);
			else if (s.StartsWith("UnityEngine.")) s = s.Substring(12);
			return s;
		}

		/// <summary>
		/// Add a child object to the specified parent and attaches the specified script to it.
		/// </summary>
		public static T AddChild<T>(this GameObject parent) where T : Component
		{
			var go = AddChild(parent);
			string name;

			if (!TypeNames.TryGetValue(typeof(T), out name) || name == null)
			{
				name = GetTypeName<T>();
				TypeNames[typeof(T)] = name;
			}
			go.name = name;
			return go.AddComponent<T>();
		}

		/// <summary>
		/// Add a child object to the specified parent and attaches the specified script to it.
		/// </summary>
		public static T AddChild<T>(this GameObject parent, bool undo) where T : Component
		{
			var go = AddChild(parent, undo);
			string name;

			if (!TypeNames.TryGetValue(typeof(T), out name) || name == null)
			{
				name = GetTypeName<T>();
				TypeNames[typeof(T)] = name;
			}
			go.name = name;
			return go.AddComponent<T>();
		}
	}
}