#region

using System;
using System.Collections.Generic;
using System.Linq;
using TMS.Common.Extensions;
using UnityEngine;

#endregion

namespace TMS.Common.Core
{
	public class InputDispatcher : ViewModel, IInputDispatcher
	{
		private readonly List<InputListenerWrapper> _listeners = new List<InputListenerWrapper>();
		private int _activeTouchInitialConsumeIndex;

		private Vector2 _prevInputPosition;

		public override void Subscribe()
		{
			base.Subscribe();
			Subscribe<InputListenerStatePayload>(OnInputListenerStateChange);
		}

		private void OnInputListenerStateChange(InputListenerStatePayload arg)
		{
			foreach (var listener in _listeners)
			{
				
			}

			switch (arg.State)
			{
				case Core.InputListenerState.Normal:
					break;

				case Core.InputListenerState.Disabled:
					break;

				case Core.InputListenerState.Locked:
					break;
			}
		}

		public void AddListener(IInputListener listener)
		{
			HandleDeadListeners();
			_listeners.Add(new InputListenerWrapper(listener));
		}

		public void RemoveListener(IInputListener listener)
		{
			HandleDeadListeners();
			_listeners.RemoveAll(item => item.IsAlive && Equals(item.Listener, listener));
		}

		private void CancelTouch(InputListenerWrapper wrapper, bool removeFromInitialList)
		{
			wrapper.Listener.OnInputCanceled(Vector2.zero);
			wrapper.ListenerState &= ~InputListenerState.InputBegan;
			if (removeFromInitialList)
			{
				wrapper.ListenerState &= ~InputListenerState.InputActive;
			}
		}

		private void HandleDeadListeners()
		{
			_listeners.RemoveAll(item => !item.IsAlive);
		}

		private IEnumerable<IInputListener> GetListeners(InputListenerState state)
		{
			var activeListeners = from item in _listeners
								  where item.IsAlive && (item.ListenerState & state) == state
								  select item.Listener;
			return activeListeners;
		}

		private InputListenerWrapper GetWrapper(IInputListener listener)
		{
			var activeListener = (from item in _listeners
								  where item.IsAlive && Equals(item.Listener, listener)
								  select item).FirstOrDefault();
			return activeListener;
		}

		protected override void Awake()
		{
			var instance = Resolve<IInputDispatcher>();
			if (instance != null)
			{
				throw new OperationCanceledException("There is another instance of this type exists.");
			}

			Register<IInputDispatcher, InputDispatcher>(this);
			//Debug.LogWarning(GetType() + " -> Register Instance");

			base.Awake();
		}

		protected void Update()
		{
			var touchActivity = Input.touchCount > 0;
			var mouseActivity = Input.mousePresent && (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0));

			if (!mouseActivity && !touchActivity)
			{
				_listeners.ForEach(item => item.ListenerState = 0);
				return;
			}

			var inputPhase = TouchPhase.Began;
			Vector2 inputPosition;
			if (touchActivity)
			{
				var touch = Input.GetTouch(0);
				inputPosition = touch.position;
				inputPhase = touch.phase;
			}
			else
			{
				inputPosition = Input.mousePosition;

				if (Input.GetMouseButtonDown(0))
				{
					inputPhase = TouchPhase.Began;
				}
				else if (Input.GetMouseButtonUp(0))
				{
					inputPhase = TouchPhase.Ended;
				}
				else if (Input.GetMouseButton(0))
				{
					inputPhase = Vector2.Distance(inputPosition, _prevInputPosition) > float.Epsilon
						? TouchPhase.Moved
						: TouchPhase.Stationary;
				}
			}

			_prevInputPosition = inputPosition;
			inputPosition = Camera.main.ScreenToWorldPoint(inputPosition);

			var hitListeners = GetHitListeners(inputPosition).CreateList();
			if (inputPhase == TouchPhase.Began)
			{
				hitListeners.ForEach(item => item.ListenerState |= InputListenerState.InputActive);
			}

			// TODO remove Linq
			var beganListeners = (from item in _listeners
								  where (item.ListenerState & InputListenerState.InputBegan) == InputListenerState.InputBegan
								  select item).CreateList();
			var cancelledListeners = beganListeners.Except(hitListeners).CreateList();

			var activeTouchInitialListeners = (from item in _listeners
											   where (item.ListenerState & InputListenerState.InputActive) == InputListenerState.InputActive
											   orderby item.Listener.GetBounds().min.z
											   select item).ToArray();
			var activeTouchInitialListenersCount = activeTouchInitialListeners.Count();

			var finalIterationIndex = _activeTouchInitialConsumeIndex == -1
				? activeTouchInitialListenersCount
				: _activeTouchInitialConsumeIndex;

			for (var i = 0; i <= finalIterationIndex && i < activeTouchInitialListenersCount; i++)
			{
				var wrapper = activeTouchInitialListeners[i];

				if (hitListeners.Contains(wrapper))
				{
					if (beganListeners.Contains(wrapper))
					{
						switch (inputPhase)
						{
							case TouchPhase.Moved:
								var isCancel = wrapper.Listener.OnInputMoved(inputPosition);
								if (isCancel)
								{
									var listenersToCancel = activeTouchInitialListeners.Skip(i + 1).CreateList();
									foreach (var listenerToCancel in listenersToCancel)
									{
										CancelTouch(listenerToCancel, true);
									}
								}
								break;

							case TouchPhase.Stationary:
								wrapper.Listener.OnInputStationary(inputPosition);
								break;

							case TouchPhase.Ended:
								wrapper.Listener.OnInputEnded(inputPosition);
								break;
						}
					}
					else
					{
						var consumed = wrapper.Listener.OnInputBegan(inputPosition);
						wrapper.ListenerState |= InputListenerState.InputBegan;
						if (!consumed) continue;
						_activeTouchInitialConsumeIndex = i;
						break;  
					}
				}
				else if (cancelledListeners.Contains(wrapper))
				{
					CancelTouch(wrapper, false);
				}
			}
			if (inputPhase != TouchPhase.Ended && inputPhase != TouchPhase.Canceled) return;

			foreach (var listener in _listeners)
			{
				listener.ListenerState = 0;
			}
			_activeTouchInitialConsumeIndex = -1;
		} // update function

		private IEnumerable<InputListenerWrapper> GetHitListeners(Vector2 inputPos)
		{
			var hitListeners = from l in _listeners
							   let bounds = l.Listener.GetBounds()
							   where bounds.Contains(new Vector3(inputPos.x, inputPos.y, bounds.center.z))
							   select l;
			return hitListeners;
		}

		[Flags]
		private enum InputListenerState
		{
			Normal = 0,
			InputBegan = 2,
			InputActive = 4,
		}

		private class InputListenerWrapper
		{
			/// <summary>
			///     Initializes a new instance of the <see cref="T:System.WeakReference" /> class, referencing the specified object.
			/// </summary>
			/// <param name="target">The object to track or null. </param>
			public InputListenerWrapper(object target)
			{
				_ref = new WeakReference(target);
			}

			public InputListenerState ListenerState { get; set; }

			private readonly WeakReference _ref;

			public bool IsAlive { get { return _ref.IsAlive && _ref.Target != null; } }

			public IInputListener Listener
			{
				get { return IsAlive ? (IInputListener)_ref.Target : null; }
			}
		}
	}

	[Flags]
	public enum InputListenerState
	{
		Normal = 1,
		Disabled = 2,
		Locked = 4,
	}

	public class InputListenerStatePayload
	{
		public int LayerId { get; set; }

		public InputListenerState State { get; set;  }

		public InputListenerStatePayload(InputListenerState state, int layerId)
		{
			State = state;
			LayerId = layerId;
		}
	}
}