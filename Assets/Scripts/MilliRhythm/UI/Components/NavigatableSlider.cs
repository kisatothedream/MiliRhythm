using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.Components
{
	public class NavigatableSlider : Slider, INavigatable
	{
		public INavigatable Up;
		public INavigatable Down;

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
		}

		public void OnNavigate(UINavigationType uiDirection)
		{
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
	}
}
