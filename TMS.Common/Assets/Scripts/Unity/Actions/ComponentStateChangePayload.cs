#region Usings

using UnityEngine;

#endregion

namespace TMS.Runtime.Unity.Actions
{
	public class ComponentStateChangePayload<T>
		where T : Component
	{
		public ComponentStateChangePayload()
		{
		}

		public ComponentStateChangePayload(T source, ComponentStateType state)
		{
			Source = source;
			State = state;
		}

		public T Source { get; set; }

		public ComponentStateType State { get; set; }
	}

	public class ComponentStateChangePayload : ComponentStateChangePayload<Component>
	{
		public ComponentStateChangePayload()
		{
		}

		public ComponentStateChangePayload(Component source, ComponentStateType state) : base(source, state)
		{
		}
	}
}