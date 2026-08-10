using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.Domain;
using MilliRhythm.Input;
using MilliRhythm.Scene;
using MilliRhythm.Scene.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.RhythmGameUI
{
	public struct ResultUIParameter
	{
		public Sprite Jacket;
		public int Perfect;
		public int Great;
		public int Good;
		public int Bad;
		public int Miss;
		public int MaxCombo;
		public int Score;
		public Rank Rank;
		public float Accuracy;
		public int AverageError;
	}

	public class ResultUI : MonoBehaviour, IUIInputListener
	{
		[SerializeField] private Button dismissArea;
		[SerializeField] private JudgeCountItem[] items;
		[SerializeField] private TextMeshProUGUI scoreText;
		[SerializeField] private TextMeshProUGUI accuracyText;
		[SerializeField] private TextMeshProUGUI averageErrorText;
		[SerializeField] private Image jacketImage;
		[SerializeField] private TextMeshProUGUI rankText;
		private bool closable;
		private MusicSelectorSceneParameter sceneParameter;
		private readonly CancellationTokenSource showScoreCts = new();

		private void Awake()
		{
			dismissArea.onClick.AddListener(TryChangeScene);
		}

		private void OnEnable()
		{
			this.RegisterUIInputListener();
		}

		private void OnDisable()
		{
			this.UnregisterUIInputListener();
		}

		private void OnDestroy()
		{
			showScoreCts?.Cancel();
			showScoreCts?.Dispose();
			dismissArea.onClick.RemoveListener(TryChangeScene);
		}

		public async UniTask ShowResultAsync(ResultUIParameter resultUIParameter, MusicSelectorSceneParameter sp)
		{
			gameObject.SetActive(true);
			jacketImage.sprite = resultUIParameter.Jacket;
			sceneParameter = sp;
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			items[0].SetCountAndShow(resultUIParameter.Perfect);
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			items[1].SetCountAndShow(resultUIParameter.Great);
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			items[2].SetCountAndShow(resultUIParameter.Good);
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			items[3].SetCountAndShow(resultUIParameter.Bad);
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			items[4].SetCountAndShow(resultUIParameter.Miss);
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			items[5].SetCountAndShow(resultUIParameter.MaxCombo);
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			accuracyText.text = $"{resultUIParameter.Accuracy * 100:000.00}%";
			averageErrorText.text = $"{(resultUIParameter.AverageError >= 0 ? "+" : "")}{resultUIParameter.AverageError:D3}ms";
			await UniTask.WaitForSeconds(1.2f, cancellationToken: showScoreCts.Token);
			scoreText.text = resultUIParameter.Score.ToString();
			await UniTask.WaitForSeconds(1.2f, cancellationToken: showScoreCts.Token);
			rankText.text = resultUIParameter.Rank.ToString();
		}

		public void Navigate(Vector2 value)
		{
		}

		public void Submit(bool value)
		{
			if (!value) return;
			TryChangeScene();
		}

		public void Cancel(bool value)
		{
			if (!value) return;
			TryChangeScene();
		}

		public void Config(bool value)
		{
			if (!value) return;
			TryChangeScene();
		}

		public void Filter(bool value)
		{
			if (!value) return;
			TryChangeScene();
		}

		private void TryChangeScene()
		{
			if (!closable)
			{
				closable = true;
				return;
			}

			if (closable)
			{
				SceneController.Instance.RequestChangeScene(sceneParameter);
			}
		}
	}
}
