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
		[SerializeField] private TextMeshProUGUI melodyScoreText;
		[SerializeField] private TextMeshProUGUI beatScoreText;

		public void ResetToPlaceholder()
		{
			jacketImage.sprite = jacketPlaceHolderSprite;
			trackNameLocalizedText.LocalizationKey = "-";
			trackVocalLocalizedText.LocalizationKey = "";
		}

		public void SetTrackInfo(Sprite jacket, string trackNameKey, string trackVocalKey, int mScore, int bScore)
		{
			jacketImage.sprite = jacket;
			trackNameLocalizedText.LocalizationKey = trackNameKey;
			trackVocalLocalizedText.LocalizationKey = trackVocalKey;
			melodyScoreText.text = mScore.ToString();
			beatScoreText.text = bScore.ToString();
		}
	}
}
