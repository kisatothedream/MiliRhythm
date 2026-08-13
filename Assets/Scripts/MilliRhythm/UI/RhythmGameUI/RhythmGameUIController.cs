using System;
using Cysharp.Threading.Tasks;
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
		[SerializeField] private Button pauseButton;
		[SerializeField] private GameStartPopup gameStartPopup;

		private void Awake()
		{
			pauseButton.onClick.AddListener(DisplayPauseUI);
			UpdateScore(0);
		}

		private void OnDestroy()
		{
			pauseButton.onClick.RemoveListener(DisplayPauseUI);
		}

		public void SetGameStartAction(Action start)
		{
			gameStartPopup.SetAction(start);
		}

		public void ShowGameStartPopup()
		{
			gameStartPopup.Display();
		}

		public void HideGameStartPopup()
		{
			gameStartPopup.Hide();
		}

		public void SetResultActions(Action pause, Action resume, Action restart, Action quit)
		{
			pauseUI.SetActions(restart, quit, pause, resume);
		}


		public void UpdateLifeGauge(int cur, int max) => lifeGaugeUI.UpdateGauge(cur, max);
		public void PlayFacePump() => lifeGaugeUI.PlayFacePump();
		public void UpdateScore(int score) => scoreUI.UpdateScore(score);

		public void ShowResultAsync(ResultUIParameter result) => resultUI.ShowResultAsync(result).Forget();

		public void DisplayPauseUI()
		{
			pauseUI.Display();
		}
	}
}
