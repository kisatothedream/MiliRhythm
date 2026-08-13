using MilliRhythm.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.Components
{
	public class NavigatableSlider : Slider, INavigatable
	{
		public RectTransform RectTransform => transform as RectTransform;

		[SerializeField] private GameObject focusIndicator;

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
			focusIndicator.SetActive(focused);
		}

		public void OnNavigate(UINavigationType uiDirection)
		{
			SfxAudioPlayer.Instance.Play(SfxType.Navigate);
			switch (uiDirection)
			{
				case UINavigationType.Left:
					value -= 0.05f;
					break;
				case UINavigationType.Right:
					value += 0.05f;
					break;
			}
		}

		public void OnSubmit()
		{
		}
	}
}
