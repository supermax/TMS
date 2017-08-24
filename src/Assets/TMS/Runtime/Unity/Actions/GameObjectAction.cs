#region Usings

using System.Collections;
using TMS.Common.Core;
using TMS.Common.Extensions;
using TMS.Common.Helpers;
using TMS.Common.Logging;
using UnityEngine;

#endregion

namespace TMS.Runtime.Unity.Actions
{
	public class GameObjectAction : MonoBehaviourBase
	{
		[SerializeField] private GameObjectActionType _action;

		[SerializeField] private bool _captureValues;

		[SerializeField] private DelegateCommand _command;

		[SerializeField] private float _delay;

		[SerializeField] private Object _target;

		[SerializeField] private GameObjectActionTrigger _trigger;

		[SerializeField] private string _triggerTag;

		protected string Id;

		public GameObjectActionTrigger Trigger
		{
			get { return _trigger; }
			protected set { _trigger = value; }
		}

		public GameObjectActionType Action
		{
			get { return _action; }
			protected set { _action = value; }
		}

		public Object Target
		{
			get { return _target; }
			protected set { _target = value; }
		}

		public DelegateCommand Command
		{
			get { return _command; }
			protected set { _command = value; }
		}

		public float Delay
		{
			get { return _delay; }
			protected set { _delay = value; }
		}

		public string TriggerTag
		{
			get { return _triggerTag; }
			protected set { _triggerTag = value; }
		}

		public bool CaptureValues
		{
			get { return _captureValues; }
			protected set { _captureValues = value; }
		}

		protected override void Awake()
		{
			base.Awake();
			DoAction(GameObjectActionTrigger.Awake);
		}

		protected override void Start()
		{
			base.Start();

			if (Id.IsNullOrEmpty())
				Id = gameObject.name;

			DoAction(GameObjectActionTrigger.Start);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			DoAction(GameObjectActionTrigger.OnDisable);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			DoAction(GameObjectActionTrigger.OnEnabled);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			DoAction(GameObjectActionTrigger.OnDestroy);
		}

		protected override void OnApplicationPause(bool pause)
		{
			OnApplicationQuit();
			DoAction(GameObjectActionTrigger.OnApplicationPause);
		}

		protected virtual void OnTriggerEnter(Collider collider)
		{
			if (Trigger != GameObjectActionTrigger.OnTriggerEnter ||
			    TriggerTag != null && !collider.gameObject.CompareTag(TriggerTag))
				return;

			DoAction(GameObjectActionTrigger.OnTriggerEnter);
		}

		protected virtual void OnCollisionEnter(Collision collision)
		{
			if (Trigger != GameObjectActionTrigger.OnCollisionEnter ||
			    TriggerTag != null && !collision.gameObject.CompareTag(TriggerTag))
				return;

			DoAction(GameObjectActionTrigger.OnCollisionEnter);
		}

		protected virtual void OnTriggerExit(Collider collider)
		{
			if (Trigger != GameObjectActionTrigger.OnTriggerExit ||
			    TriggerTag != null && !collider.gameObject.CompareTag(TriggerTag))
				return;

			DoAction(GameObjectActionTrigger.OnTriggerExit);
		}

		protected virtual void OnCollisionExit(Collision collision)
		{
			if (Trigger != GameObjectActionTrigger.OnCollisionExit ||
			    TriggerTag != null && !collision.gameObject.CompareTag(TriggerTag))
				return;

			DoAction(GameObjectActionTrigger.OnCollisionExit);
		}

		protected virtual void DoAction(GameObjectActionTrigger trigger)
		{
			if (trigger != Trigger)
				return;
			DoAction();
		}

		protected virtual Object GetTarget()
		{
			var target = Target == DefaultValues.Default.ObjectNull ? this : Target;
			return target;
		}

		public virtual void DoAction()
		{
			if (Delay <= 0f)
			{
				DoActionInternal();
				return;
			}

			StartCoroutine(DoActionRoutine());
		}

		protected virtual IEnumerator DoActionRoutine()
		{
			yield return new WaitForSeconds(Delay);
			DoActionInternal();
		}

		protected virtual void DoActionInternal()
		{
			var target = GetTarget();
			switch (Action)
			{
				case GameObjectActionType.Activate:
					SetActiveState(target, true);
					break;

				case GameObjectActionType.Disactivate:
					SetActiveState(target, false);
					break;

				case GameObjectActionType.Enable:
					SetEnabledState(target, true);
					break;

				case GameObjectActionType.Disable:
					SetEnabledState(target, false);
					break;

				case GameObjectActionType.Destroy:
					DestroyObject(target);
					break;

				case GameObjectActionType.ExecuteCommand:
					ExecuteCommand(Command, target);
					break;
			}
			Log("Target: {0}, Trigger: {1}, Action: {2}\r\nParent: {3}", target, Trigger, Action, gameObject);
		}

		protected static void ExecuteCommand(IDelegateCommand cmd, object parameter)
		{
			if (cmd == null || !cmd.CanExecute(parameter))
				return;

			cmd.Execute(parameter);
		}

		protected static void SetActiveState(Object target, bool value)
		{
			var go = target as GameObject;
			if (go != null)
				go.SetActive(value);

			var mb = target as Component;
			if (mb != null)
				mb.gameObject.SetActive(value);
		}

		protected static void SetEnabledState(Object target, bool value)
		{
			var mb = target as Behaviour;
			if (mb != null)
				mb.enabled = value;
		}

		protected void Log(string format, params object[] args)
		{
			if (LogSeverity != LogSeverityType.Normal)
				return;

			var log = string.Format(format, args);
			Debug.LogFormat("[{0}] {1}", Id, log);
		}
	}
}