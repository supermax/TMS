#region Usings

using TMS.Common.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

#endregion

namespace TMS.Runtime.Unity.Actions
{
	[RequireComponent(typeof(Button))]
	public class InputButtonHelper : MonoBehaviourBase
	{
		[SerializeField] private string _inputButtonName;

		[SerializeField] private KeyCode _inputKeyCode = KeyCode.None;

		[SerializeField] private InputButtonTrigger _inputTrigger;

		[SerializeField] private Button _sourceButton;

		public virtual Button SourceButton
		{
			get { return _sourceButton; }
		}

		public virtual InputButtonTrigger InputTrigger
		{
			get { return _inputTrigger; }
			set { _inputTrigger = value; }
		}

		public virtual KeyCode InputKeyCode
		{
			get { return _inputKeyCode; }
			set { _inputKeyCode = value; }
		}

		public virtual string InputButtonName
		{
			get { return _inputButtonName; }
			set { _inputButtonName = value; }
		}

		protected override void Start()
		{
			base.Start();

			if (_sourceButton == null)
				_sourceButton = GetComponent<Button>();
		}

		private void Update()
		{
			switch (InputTrigger)
			{
				case InputButtonTrigger.KeyDown:
					if (!Input.GetKeyDown(InputKeyCode)) return;
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