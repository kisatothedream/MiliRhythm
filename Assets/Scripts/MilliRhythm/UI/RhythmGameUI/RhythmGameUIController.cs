using System;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.Domain;
using MilliRhythm.Scene.Contracts;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.RhythmGameUI
{
	public class RhythmGameUIController : MonoBehaviour
	{
		[SerializeField] private LifeGaugeUI lifeGaugeUI;
		[SerializeField] private ScoreUI scoreUI;
		[SerializeField] private ResultUI resultUI;
		[SerializeField] private PauseUI pauseUI;

		[SerializeField] private Image trackProgressFill;
		[SerializeField] private Button pauseButton;

		[SerializeField] private TimingCalibrator timingCalibrator;

		private void Awake()
		{
			pauseButton.onClick.AddListener(DisplayPauseUI);
			UpdateScore(0, 0);
		}

		private void OnDestroy()
		{
			pauseButton.onClick.RemoveListener(DisplayPauseUI);
		}

		public void UpdateLifeGauge(int cur, int max) => lifeGaugeUI.UpdateGauge(cur, max);
		public void UpdateScore(int score, float accuracy) => scoreUI.UpdateScore(score, accuracy);

		public void ShowResultAsync() => resultUI.ShowResultAsync(new ResultUIParameter()).Forget();

		public void DisplayPauseUI()
		{
			pauseUI.Display();
		}
	}
}
