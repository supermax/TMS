namespace TMS.Runtime.Unity.Actions
{
	public class GameObjectActionToggle : GameObjectAction
	{
		protected override void DoActionInternal()
		{
			switch (Action)
			{
				case GameObjectActionType.Activate:
					Action = GameObjectActionType.Disactivate;
					break;

				case GameObjectActionType.Disactivate:
					Action = GameObjectActionType.Activate;
					break;

				case GameObjectActionType.Enable:
					Action = GameObjectActionType.Disable;
					break;

				case GameObjectActionType.Disable:
					Action = GameObjectActionType.Enable;
					break;

				//case GameObjectActionType.Destroy:
				//	Action = GameObjectActionType. // TODO create ??
				//	break;			
			}
			base.DoActionInternal();
		}
	}
}