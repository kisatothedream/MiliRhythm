using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.RhythmGameUI
{
	public class ScoreUI : MonoBehaviour
	{
		[SerializeField] private Image accuracyBarFill;
		[SerializeField] private TextMeshProUGUI scoreText;
		[SerializeField] private TextMeshProUGUI accuracyText;

		public void UpdateScore(int score, float accuracy)
		{
			accuracyBarFill.fillAmount = accuracy;
			scoreText.text = score.ToString("D7");
			accuracyText.text = $"{accuracy * 100:0.00}%";
		}
	}
}
