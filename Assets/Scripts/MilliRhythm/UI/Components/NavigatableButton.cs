using UnityEngine;

namespace MilliRhythm.UI.Components
{
	public class NavigatableButton : MonoBehaviour, INavigatable
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
	}
}
