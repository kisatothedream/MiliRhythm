using System;
using MilliRhythm.Input;
using UnityEngine;

namespace MilliRhythm.UI.RhythmGameUI
{
	public class GameStartPopup : MonoBehaviour, IUIInputListener
	{
		private Action startAction;

		private void OnEnable()
		{
			this.RegisterUIInputListener();
		}

		private void OnDisable()
		{
			this.UnregisterUIInputListener();
		}

		public void SetAction(Action start)
		{
			startAction = start;
		}

		public void Display()
		{
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}

		public void Navigate(Vector2 value)
		{
		}

		public void Submit(bool value)
		{
			if (value)
			{
				startAction?.Invoke();
			}
		}

		public void Cancel(bool value)
		{
		}

		public void Config(bool value)
		{
		}

		public void Filter(bool value)
		{
		}

		public void AnyKey(bool value)
		{
		}
	}
}
