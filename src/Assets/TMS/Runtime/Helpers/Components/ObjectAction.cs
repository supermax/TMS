using System.Collections;
using System.Collections.Generic;
using TMS.Common.Core;
using UnityEngine;

namespace CFX.Breakout.Test.Common.Helpers
{
    public abstract class ObjectAction : MonoBehaviourBase 
	{
		[SerializeField]
		private ObjectActionTrigger _actionTrigger;

		public virtual ObjectActionTrigger ActionTrigger
		{
			get { return _actionTrigger; }
			set { _actionTrigger = value; }
		}

		public abstract void DoAction();

		protected override void Awake()
		{
			base.Awake();			
			if(ActionTrigger != ObjectActionTrigger.Awake) return;
			DoAction();
		}

		protected override void Start()
        {
            base.Start();
            if (ActionTrigger != ObjectActionTrigger.Start) return;
            DoAction();
        }

		protected override void OnEnable()
        {
            base.OnEnable();
            if (ActionTrigger != ObjectActionTrigger.OnEnable) return;
            DoAction();
        }

		protected override void OnDisable()
        {
            base.OnDisable();
            if (ActionTrigger != ObjectActionTrigger.OnDisable) return;
            DoAction();
        }

		protected override void OnDestroy()
        {
            base.OnDestroy();
            if (ActionTrigger != ObjectActionTrigger.OnDestroy) return;
            DoAction();
        }
	}
}
