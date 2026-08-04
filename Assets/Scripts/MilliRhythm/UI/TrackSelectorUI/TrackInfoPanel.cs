using MilliRhythm.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.TrackSelectorUI
{
	public class TrackInfoPanel : MonoBehaviour
	{
		[SerializeField] private Sprite jacketPlaceHolderSprite;
		[SerializeField] private Image jacketImage;
		[SerializeField] private LocalizedText trackNameLocalizedText;
		[SerializeField] private LocalizedText trackVocalLocalizedText;

		public void ResetToPlaceholder()
		{
			jacketImage.sprite = jacketPlaceHolderSprite;
			trackNameLocalizedText.LocalizationKey = "-";
			trackVocalLocalizedText.LocalizationKey = "";
		}

		public void SetTrackInfo(Sprite jacket, string trackNameKey, string trackVocalKey)
		{
			jacketImage.sprite = jacket;
			trackNameLocalizedText.LocalizationKey = trackNameKey;
			trackVocalLocalizedText.LocalizationKey = trackVocalKey;
		}
	}
}
