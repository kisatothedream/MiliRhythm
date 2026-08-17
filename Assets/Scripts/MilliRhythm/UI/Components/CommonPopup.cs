using MilliRhythm.Audio;
using UnityEngine;

namespace MilliRhythm.UI.Components
{
	public class CommonPopup : PopupUIBase<CommonPopupParameter, DefaultPopupResponse, DefaultPopupResultPayload>
	{
		[SerializeField] private LocalizedText titleText;
		[SerializeField] private LocalizedText contentText;

		protected override void Set()
		{
			response = new DefaultPopupResponse()
			{
				Payload = new(),
			};
			titleText.LocalizationKey = parameter.TitleTextKey;
			contentText.LocalizationKey = parameter.ContentTextKey;
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

	public class CommonPopupParameter : PopupParameterBase
	{
		public string TitleTextKey;
		public string ContentTextKey;
	}
}
