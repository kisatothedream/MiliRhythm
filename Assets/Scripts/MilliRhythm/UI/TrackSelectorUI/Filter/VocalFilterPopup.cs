using MilliRhythm.UI.Components;
using UnityEngine;

namespace MilliRhythm.UI.TrackSelectorUI.Filter
{
	public class VocalFilterPopup : PopupUIBase<VocalFilterPopupParameter, VocalFilterPopupResponse, VocalFilterPopupPayload>
	{
		
		protected override void Set()
		{
		}

		public override void OnNavigate(Vector2 value)
		{
		}

		public override void OnSubmit(bool value)
		{
		}

		public override void OnCancel(bool value)
		{
		}
	}

	public class VocalFilterPopupParameter : PopupParameterBase
	{
	}

	public class VocalFilterPopupResponse : PopupResponse<VocalFilterPopupPayload>
	{
	}

	public class VocalFilterPopupPayload : PopupResultPayload
	{
	}
}
