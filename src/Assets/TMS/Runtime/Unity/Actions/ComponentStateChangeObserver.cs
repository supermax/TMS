#region Usings

using TMS.Common.Messaging;
using UnityEngine;

#endregion

namespace TMS.Runtime.Unity.Actions
{
	public class ComponentStateChangeObserver : ComponentAction
	{
		protected virtual void OnStateChanged(ComponentStateType state)
		{
			var target = GetTarget() as Component;
			Messenger.Default.Publish(new ComponentStateChangePayload(target, state));
		}

		protected override void Awake()
		{
			base.Awake();
			OnStateChanged(ComponentStateType.Created);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			OnStateChanged(ComponentStateType.Enabled);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			OnStateChanged(ComponentStateType.Disabled);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			OnStateChanged(ComponentStateType.Destroyed);
		}
	}
}