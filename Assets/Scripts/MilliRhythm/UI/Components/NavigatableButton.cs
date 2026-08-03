using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.Components
{
	public class NavigatableButton : Button, INavigatable
	{
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
			transform.localScale = (focused ? 1.2f : 1) * Vector3.one;
		}

		public void OnNavigate(UINavigationType direction)
		{
		}

		public void OnSubmit()
		{
			onClick?.Invoke();
		}
	}
}
