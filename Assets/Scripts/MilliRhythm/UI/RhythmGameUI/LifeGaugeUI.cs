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
		private Tween pumpTween;

		private void Awake()
		{
			pumpTween = faceIconRect.DOScale(1, 0.2f)
				.From(1.4f)
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
		}

		public void PlayFacePump()
		{
			pumpTween.Restart();
		}
	}
}
