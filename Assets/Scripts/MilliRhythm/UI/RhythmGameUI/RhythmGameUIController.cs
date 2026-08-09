using Cysharp.Threading.Tasks;
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
		[SerializeField] private Image trackProgressFill;
		[SerializeField] private Button quitButton;

		private void Awake()
		{
			quitButton.onClick.AddListener(OnQuitButtonAction);
			UpdateScore(0, 0);
		}

		private void OnDestroy()
		{
			quitButton.onClick.RemoveListener(OnQuitButtonAction);
		}

		public void UpdateLifeGauge(int cur, int max) => lifeGaugeUI.UpdateGauge(cur, max);
		public void UpdateScore(int score, float accuracy) => scoreUI.UpdateScore(score, accuracy);
		public void UpdateTrackProgress(float progress) => trackProgressFill.fillAmount = progress;

		public void ShowResultAsync(Sprite jacket, int perfect, int great, int good, int bad, int miss, int maxCombo, int score, MusicSelectorSceneParameter param) =>
			resultUI.ShowResultAsync(jacket, perfect, great, good, bad, miss, maxCombo, score, param).Forget();

		private void OnQuitButtonAction()
		{
		}
	}
}
