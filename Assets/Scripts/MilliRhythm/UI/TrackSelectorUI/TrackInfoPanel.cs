using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.TrackSelectorUI
{
	public class TrackInfoPanel : MonoBehaviour
	{
		[SerializeField] private Sprite jacketPlaceHolderSprite;
		[SerializeField] private Image jacketImage;
		[SerializeField] private TextMeshProUGUI trackNameText;
		[SerializeField] private TextMeshProUGUI trackVocalText;

		public void ResetToPlaceholder()
		{
			jacketImage.sprite = jacketPlaceHolderSprite;
			trackNameText.text = "-";
			trackVocalText.text = "";
		}

		public void SetTrackInfo(Sprite jacket, string trackName, string trackVocal)
		{
			jacketImage.sprite = jacket;
			trackNameText.text = trackName;
			trackVocalText.text = trackVocal;
		}
	}
}
