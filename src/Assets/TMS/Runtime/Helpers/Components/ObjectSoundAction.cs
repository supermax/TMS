using System;
using System.Collections;
using System.Collections.Generic;
using TMS.Common.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace CFX.Breakout.Test.Common.Helpers
{
    [RequireComponent(typeof(AudioSource))]
    public class ObjectSoundAction : ObjectAction
    {
		private AudioSource _sfx;

		[SerializeField]
		private string _colliderName;
		
		protected override void Awake()
        {
            base.Awake();

			Assert.IsNotNull(_colliderName, "Collider Name is NULL");
            
			_sfx = GetComponent<AudioSource>();
            Assert.IsNotNull(_sfx, "Audio Source is NULL");
        }

        public override void DoAction()
        {
			_sfx.Play();
        }

		void OnCollisionEnter(Collision other)
        {
			if(ActionTrigger != ObjectActionTrigger.OnCollisionEnter) return;
            if (!other.gameObject.CompareTag(_colliderName)) return;

            DoAction();
        }
    }
}