#region Usings

using UnityEngine;

#endregion

namespace TMS.Runtime.Unity.Actions
{
	public class GameObjectTransformAction : GameObjectAction
	{
		public Transform Pivot;

		public Vector3 Scalar;

		protected override void DoActionInternal()
		{
			base.DoActionInternal();

			if (Action != GameObjectActionType.Transform)
				return;

			var target = GetTarget() as GameObject;
			if (target == null)
				return;

			var delta = Pivot.transform.position - target.transform.position;
			target.transform.position += Vector3.Scale(delta, Scalar);
		}
	}
}