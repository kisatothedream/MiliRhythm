using System;
using MilliRhythm.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.Components
{
	public class NavigatableToggleGroup : ToggleGroup, INavigatable
	{
		public RectTransform RectTransform => transform as RectTransform;
		[SerializeField] private GameObject focusedObject;

		private Toggle[] toggles;
		private Toggle current;

		protected override void OnEnable()
		{
			base.OnEnable();
			toggles = GetComponentsInChildren<Toggle>();
		}

		public void Focus()
		{
			ApplyFocusState(true);
		}

		public void Unfocus()
		{
			ApplyFocusState(false);
		}

		public void ApplyFocusState(bool focused)
		{
			focusedObject.SetActive(focused);
		}

		public void OnNavigate(UINavigationType direction)
		{
			SfxAudioPlayer.Instance.Play(SfxType.Navigate);
			switch (direction)
			{
				case UINavigationType.Left:
					Select(FindCurrentItem() - 1);
					break;
				case UINavigationType.Right:
					Select(FindCurrentItem() + 1);
					break;
			}
		}

		public void OnSubmit()
		{
		}

		private int FindCurrentItem() => Array.FindIndex(toggles, toggle => toggle.isOn);

		private void Select(int index)
		{
			var next = Math.Clamp(index, 0, toggles.Length - 1);
			toggles[next].isOn = true;
		}
	}
}
