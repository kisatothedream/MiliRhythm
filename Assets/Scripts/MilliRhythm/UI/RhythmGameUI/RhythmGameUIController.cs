using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.RhythmGameUI
{
	public class RhythmGameUIController : MonoBehaviour
	{
		[SerializeField] private LifeGaugeUI lifeGaugeUI;
		[SerializeField] private ScoreUI scoreUI;
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

		private void OnQuitButtonAction()
		{
		}
	}
}
