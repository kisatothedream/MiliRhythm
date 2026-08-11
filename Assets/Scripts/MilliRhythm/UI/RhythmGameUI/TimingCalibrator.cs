using TMPro;
using UnityEngine;

namespace MilliRhythm.UI.RhythmGameUI
{
	public class TimingCalibrator : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI averageErrorValue;
		[SerializeField] private RectTransform pin;

		public void SetPosition(double average)
		{
			pin.anchoredPosition = new Vector2(pin.anchoredPosition.x, (int)average);
			averageErrorValue.text = $"{(average >= 0 ? "+" : "")}{(int)average:D3}ms";
		}
	}
}
