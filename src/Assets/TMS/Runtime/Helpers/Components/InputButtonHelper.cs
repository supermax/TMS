using System.Collections;
using System.Collections.Generic;
using TMS.Common.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace CFX.Breakout.Test.Common.Helpers
{
    [RequireComponent(typeof(Button))]
	public class InputButtonHelper : MonoBehaviourBase
	{
		[SerializeField]
		private Button _sourceButton;

		public virtual Button SourceButton
		{
			get { return _sourceButton; }
		}

		[SerializeField]
		private InputButtonTrigger _inputTrigger;

		public virtual InputButtonTrigger InputTrigger
		{
			get { return _inputTrigger; }
			set { _inputTrigger = value; }
		}

		[SerializeField]
		private KeyCode _inputKeyCode = KeyCode.None;

		public virtual KeyCode InputKeyCode
		{
			get { return _inputKeyCode; }
			set { _inputKeyCode = value; }
		}

		[SerializeField]
        private string _inputButtonName;

        public virtual string InputButtonName
        {
            get { return _inputButtonName; }
            set { _inputButtonName = value; }
        }

		protected override void Start()
		{
			base.Start();
			
			if(_sourceButton == null)
			{
				_sourceButton = GetComponent<Button>();
			}
		}

		void Update()
		{
			switch (InputTrigger)
			{
				case InputButtonTrigger.KeyDown:
					if(!Input.GetKeyDown(InputKeyCode)) return;
					break;

				case InputButtonTrigger.KeyUp:
                    if (!Input.GetKeyUp(InputKeyCode)) return;
                    break;

				case InputButtonTrigger.ButtonDown:
                    if (!Input.GetButtonDown(InputButtonName)) return;
                    break;

				case InputButtonTrigger.ButtonUp:
                    if (!Input.GetButtonUp(InputButtonName)) return;
                    break;

				default:
					return;
			}
			InvokeButtonActions();
		}		

		public virtual void InvokeButtonActions()
		{
			Assert.IsNotNull(_sourceButton, "Source Button is NULL");
			Assert.IsNotNull(_sourceButton.onClick, "Source Button -> 'onClick' field is NULL");

			_sourceButton.onClick.Invoke();

			Debug.LogFormat("Trigger '{0}' -> '{1}' action on button '{2}'", InputTrigger, 
								InputButtonName ?? InputKeyCode.ToString(), _sourceButton);
		}
	}
}
