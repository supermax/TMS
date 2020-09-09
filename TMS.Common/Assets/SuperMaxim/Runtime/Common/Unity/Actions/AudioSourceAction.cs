#region Usings

using UnityEngine;
using UnityEngine.Assertions;

#endregion

namespace TMS.Runtime.Unity.Actions
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioSourceAction : GameObjectAction
	{
		private AudioSource _sfx;

		protected override void Awake()
		{
			_sfx = GetComponent<AudioSource>();
			Assert.IsNotNull(_sfx, "Audio Source is NULL");

			base.Awake();
		}

		protected override void DoActionInternal()
		{
			base.DoActionInternal();
			_sfx.Play();
		}
	}
}