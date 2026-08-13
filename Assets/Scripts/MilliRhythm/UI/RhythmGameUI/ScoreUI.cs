using System;
using TMPro;
using UnityEngine;

namespace MilliRhythm.UI.RhythmGameUI
{
	public class ScoreUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI scoreText;
		[SerializeField] private TextMeshProUGUI accuracyText;

		public void UpdateScore(int score)
		{
			var clampedScore = Math.Clamp(score, 0, 1000000);
			scoreText.text = clampedScore.ToString("D7");
		}
	}
}
