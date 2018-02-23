#region Usings

using System;
using UnityEngine;

#endregion

namespace TMS.Runtime.Unity.Actions
{
	public class AnimatorAction : ComponentAction
	{
		[SerializeField] public AnimatorParameter[] Params;

		protected override void DoActionInternal()
		{
			base.DoActionInternal();

			var animator = GetTarget() as Animator;

			foreach (var item in Params)
				// TODO add safety checks and etc
				switch (item.Type)
				{
					case AnimatorParameterType.Int:
						animator.SetInteger(item.Name, int.Parse(item.Value));
						break;

					case AnimatorParameterType.Bool:
						animator.SetBool(item.Name, bool.Parse(item.Value));
						break;

					case AnimatorParameterType.Float:
						animator.SetFloat(item.Name, float.Parse(item.Value));
						break;

					case AnimatorParameterType.Trigger:
						animator.SetTrigger(item.Name);
						break;
				}
		}
	}

	[Serializable]
	public class AnimatorParameter
	{
		[SerializeField] public string Name;

		[SerializeField] public AnimatorParameterType Type;

		[SerializeField] public string Value;
	}

	[Serializable]
	public enum AnimatorParameterType
	{
		Int,
		Bool,
		Float,
		Trigger
	}
}