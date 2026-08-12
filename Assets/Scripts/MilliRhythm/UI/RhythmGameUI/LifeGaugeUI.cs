using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.RhythmGameUI
{
	public class LifeGaugeUI : MonoBehaviour
	{
		[SerializeField] private Image gaugeFill;
		[SerializeField] private Image faceIconImage;
		[SerializeField] private RectTransform faceIconRect;

		public void UpdateGauge(int cur, int max)
		{
			gaugeFill.fillAmount = (float)cur / (float)max;
			var anchor = faceIconRect.anchorMin;
			anchor.y = (float)cur / max;

			faceIconRect.anchorMin = anchor;
			faceIconRect.anchorMax = anchor;
		}
	}
}
