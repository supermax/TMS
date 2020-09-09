namespace TMS.Common.Core
{
	public interface IInputDispatcher
	{
		void AddListener(IInputListener listener);

		void RemoveListener(IInputListener listener);
	}
}