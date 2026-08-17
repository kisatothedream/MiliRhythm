using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.RhythmGameUI
{
	public class LifeGaugeUI : MonoBehaviour
	{
		[SerializeField] private Image gaugeFill;

		public void UpdateGauge(int cur, int max)
		{
			gaugeFill.fillAmount = cur / (float)max;
		}
	}
}
