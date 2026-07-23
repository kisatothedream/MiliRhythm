using UnityEngine;

namespace MilliRhythm.Input
{
	public interface IUIInputListener
	{
		public void OnNavigate(Vector2 value);
		public void OnSubmit(bool value);
		public void OnCancel(bool value);
	}

	public static class IUIInputListenerExtensions
	{
		public static void RegisterUIInputListener(this IUIInputListener inputListener)
		{
			UIInputManager.Instance.Register(inputListener);
		}

		public static void UnregisterUIInputListener(this IUIInputListener inputListener)
		{
			UIInputManager.Instance.Unregister(inputListener);
		}
	}
}
