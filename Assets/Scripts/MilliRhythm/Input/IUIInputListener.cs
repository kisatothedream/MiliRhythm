using UnityEngine;

namespace MilliRhythm.Input
{
	public interface IUIInputListener
	{
		public void Navigate(Vector2 value);
		public void Submit(bool value);
		public void Cancel(bool value);
		public void View(bool value);
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
