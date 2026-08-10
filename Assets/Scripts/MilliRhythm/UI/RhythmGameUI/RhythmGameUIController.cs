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

		private Action restartAction;
		private Action quitAction;

		private void Awake()
		{
			pauseButton.onClick.AddListener(DisplayPauseUI);
			UpdateScore(0, 0);
		}

		private void OnDestroy()
		{
			pauseButton.onClick.RemoveListener(DisplayPauseUI);
		}

		public void InitializeCurrentTrackContext(Action restartAction, Action quitAction)
		{
			this.restartAction = restartAction;
			this.quitAction = quitAction;
		}

		public void UpdateLifeGauge(int cur, int max) => lifeGaugeUI.UpdateGauge(cur, max);
		public void UpdateScore(int score, float accuracy) => scoreUI.UpdateScore(score, accuracy);
		public void UpdateTrackProgress(float progress) => trackProgressFill.fillAmount = progress;

		public void ShowResultAsync(Sprite jacket, int perfect, int great, int good, int bad, int miss, int maxCombo, int score, Rank rank, float accuracy, double averageError,
			MusicSelectorSceneParameter param) =>
			resultUI.ShowResultAsync(new ResultUIParameter()
			{
				Jacket = jacket, Perfect = perfect, Great = great, Good = good,
				Bad = bad,
				Miss = miss,
				MaxCombo = maxCombo,
				Score = score,
				Rank = rank,
				Accuracy = accuracy,
				AverageError = (int)averageError,
			}, param).Forget();

		public void DisplayPauseUI()
		{
			pauseUI.Display(restartAction, quitAction);
		}

		public void SetTimingErrorValue(double average) => timingCalibrator.SetPosition(average);
	}
}
