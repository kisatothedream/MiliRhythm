using MilliRhythm.Data.Domain;
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
		[SerializeField] private TextMeshProUGUI melodyRankText;
		[SerializeField] private TextMeshProUGUI beatRankText;

		public void ResetToPlaceholder()
		{
			jacketImage.sprite = jacketPlaceHolderSprite;
			trackNameLocalizedText.LocalizationKey = "-";
			trackVocalLocalizedText.LocalizationKey = "";
			melodyScoreText.text = "0";
			beatScoreText.text = "0";
			melodyRankText.text = "-";
			beatRankText.text = "-";
		}

		public void SetTrackInfo(Sprite jacket, string trackNameKey, string trackVocalKey, int mScore, int bScore, Rank mRank, Rank bRank)
		{
			jacketImage.sprite = jacket;
			trackNameLocalizedText.LocalizationKey = trackNameKey;
			trackVocalLocalizedText.LocalizationKey = trackVocalKey;
			melodyScoreText.text = mScore.ToString();
			beatScoreText.text = bScore.ToString();
			melodyRankText.text = mRank.ToString();
			beatRankText.text = bRank.ToString();
		}
	}
}
