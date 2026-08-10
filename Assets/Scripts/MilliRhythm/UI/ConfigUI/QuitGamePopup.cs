using MilliRhythm.UI.Components;
using UnityEngine;

namespace MilliRhythm.UI.ConfigUI
{
	public class QuitGamePopup : PopupUIBase<DefaultPopupParameter, DefaultPopupResponse, DefaultPopupResultPayload>
	{
		protected override void Set()
		{
			response = new DefaultPopupResponse();
		}

		public override void OnNavigate(Vector2 value)
		{
		}

		public override void OnSubmit()
		{
			Confirm();
		}

		public override void OnCancel()
		{
			Cancel();
		}
	}
}
