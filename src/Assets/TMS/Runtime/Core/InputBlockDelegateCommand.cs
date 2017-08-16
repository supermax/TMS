namespace TMS.Common.Core
{
	public class InputBlockDelegateCommand : DelegateCommand
	{
		protected override void Awake()
		{
			Init(o => { });
			//Debug.Log("InputBlockDelegateCommand->Execute()"));
			base.Awake();
		}
	}
}