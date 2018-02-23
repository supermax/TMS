#define UNITY3D

#if UNITY3D && UNITY_EDITOR
using System.Text;
using UnityEngine;

namespace TMS.Common.Modularity.Unity
{
	public class SingletonsListComponent : MonoBehaviour
	{
		/*
		[SerializeField] private Dictionary<Type, TypeMappingInfo> _mappings;

		internal Dictionary<Type, TypeMappingInfo> Mappings
		{
			get { return _mappings; }
			set { _mappings = value; }
		}
		*/

#region TEMP
		public string Singletons;

		private StringBuilder _builder = new StringBuilder();

		public void Add(string name)
		{
			_builder.AppendLine(name);
			UpdateList();
		}

		public void Remove(string name)
		{
			_builder.Replace(name, string.Empty);
			UpdateList();
		}

		private void UpdateList()
		{
			Singletons = _builder.ToString().Trim();
		}
		#endregion
	}

	/*
	[CustomPropertyDrawer(typeof(MappingInfo))]
	internal class MappingInfoPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			var amountRect = new Rect(position.x, position.y, 30, position.height);
			var unitRect = new Rect(position.x + 35, position.y, 50, position.height);
			var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

			EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);
			EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("unit"), GUIContent.none);
			EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);

			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();

			base.OnGUI(position, property, label);
		}
	}

	[CustomPropertyDrawer(typeof(TypeMappingInfo))]
	internal class TypeMappingInfoPropertyDrawer : PropertyDrawer
	{
		
	}

	[CustomPropertyDrawer(typeof(Dictionary<Type, TypeMappingInfo>))]
	internal class TypeMappingInfoDictionaryPropertyDrawer : PropertyDrawer
	{
		
	}
	*/
}
#endif