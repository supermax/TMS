using UnityEngine;

namespace TMS.Common.Core
{
	public interface IInputListener
	{
		bool OnInputBegan(Vector2 position);
		bool OnInputMoved(Vector2 position);
		void OnInputEnded(Vector2 position);
		void OnInputCanceled(Vector2 position);
		void OnInputStationary(Vector2 position);
		Bounds GetBounds();
	}
}
