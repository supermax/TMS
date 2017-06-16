namespace TMS.Common.Core
{
	public class ElementStateManagerBase : Observable
	{
		public virtual IElementStateManagerAdapter Adapter { get; set; }

		public ElementStateManagerBase()
		{
			Adapter = new ElementStateManagerAdapter(this);
		}
	}

	//public class LobbyButtonStateManager : ElementStateManagerBase
	//{
	//	public LobbyButtonStateManager()
	//	{
	//		Adapter = new LobbyButtonStateManagerAdapter(this);
	//	}
	//}

	//public class LobbyButtonStateManagerAdapter : ElementStateManagerAdapter
	//{
	//	public ElementStateManagerBase Owner { get; private set; }

	//	public LobbyButtonStateManagerAdapter(ElementStateManagerBase owner)
	//		: base(owner)
	//	{

	//	}

	//	public override void UpdateState(ElementStates state)
	//	{
	//		//base.UpdateState(state);
	//	}
	//}

	public class ElementStateManagerAdapter : IElementStateManagerAdapter
	{
		public ElementStateManagerBase Owner { get; private set; }

		public ElementStateManagerAdapter(ElementStateManagerBase owner)
		{
			Owner = owner;
		}

		public virtual void UpdateState(ElementStates state)
		{
			// 
		}
	}

	public interface IElementStateManagerAdapter
	{
		void UpdateState(ElementStates state);
	}

	public enum ElementStates
	{
		Normal,
		Pressed,
		Disabled,
		Locked,
		Hiden
	}
}