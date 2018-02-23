#region Usings

using UnityEngine;

#endregion

namespace TMS.Runtime.Unity.Actions
{
	public class ComponentAction : GameObjectAction
	{
		public string ComponentType;

		protected override Object GetTarget()
		{
			var target = base.GetTarget();
			if (target == null)
			{
				var component = GetComponent(ComponentType);
				return component;
			}

			if (target.GetType().Name.Contains(ComponentType))
				return target;

			var go = target as GameObject;
			if (go == null)
				return null;

			var component1 = go.GetComponent(ComponentType);
			return component1;
		}
	}
}