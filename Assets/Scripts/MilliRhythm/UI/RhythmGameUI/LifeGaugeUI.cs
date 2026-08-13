using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.RhythmGameUI
{
	public class LifeGaugeUI : MonoBehaviour
	{
		[SerializeField] private Image gaugeFill;
		[SerializeField] private Image faceIconImage;
		[SerializeField] private RectTransform faceIconRect;
		[SerializeField] private Sprite[] faces;
		private Tween pumpTween;

		private void Awake()
		{
			pumpTween = faceIconRect.DOScale(1, 0.12f)
				.From(1.3f)
				.SetEase(Ease.OutQuad)
				.Pause()
				.SetAutoKill(false);
		}

		public void UpdateGauge(int cur, int max)
		{
			gaugeFill.fillAmount = (float)cur / (float)max;
			var anchor = faceIconRect.anchorMin;
			anchor.x = (float)cur / max;

			faceIconRect.anchorMin = anchor;
			faceIconRect.anchorMax = anchor;
			switch (cur)
			{
				case 1: 
				case 2: faceIconImage.sprite = faces[2]; break;
				case 3:
				case 4: faceIconImage.sprite = faces[0]; break;
				case 5: faceIconImage.sprite = faces[1]; break;
			}
		}

		public void PlayFacePump()
		{
			pumpTween.Restart();
		}
	}
}
