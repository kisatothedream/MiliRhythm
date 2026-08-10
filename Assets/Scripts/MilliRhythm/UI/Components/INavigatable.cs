using UnityEngine;

namespace MilliRhythm.UI.Components
{
	public interface INavigatable
	{
		RectTransform RectTransform { get; }

		void Focus();
		void Unfocus();
		void ApplyFocusState(bool focused);
		void OnNavigate(UINavigationType direction);
		void OnSubmit();
	}
}
