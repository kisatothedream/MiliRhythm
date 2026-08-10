using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MilliRhythm.Input;
using MilliRhythm.Scene;
using MilliRhythm.Scene.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.RhythmGameUI
{
	public class ResultUI : MonoBehaviour, IUIInputListener
	{
		[SerializeField] private Button dismissArea;
		[SerializeField] private JudgeCountItem[] items;
		[SerializeField] private TextMeshProUGUI scoreText;
		[SerializeField] private Image jacketImage;
		private bool closable;
		private MusicSelectorSceneParameter sceneParameter;
		private readonly CancellationTokenSource showScoreCts = new();

		private void Awake()
		{
			dismissArea.onClick.AddListener(TryChangeScene);
			gameObject.SetActive(false);
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
			dismissArea.onClick.RemoveListener(TryChangeScene);
		}

		public async UniTask ShowResultAsync(Sprite jacket, int perfect, int great, int good, int bad, int miss, int maxCombo, int score,
			MusicSelectorSceneParameter param)
		{
			gameObject.SetActive(true);
			jacketImage.sprite = jacket;
			sceneParameter = param;
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			items[0].SetCountAndShow(perfect);
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			items[1].SetCountAndShow(great);
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			items[2].SetCountAndShow(good);
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			items[3].SetCountAndShow(bad);
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			items[4].SetCountAndShow(miss);
			await UniTask.WaitForSeconds(0.4f, cancellationToken: showScoreCts.Token);
			items[5].SetCountAndShow(maxCombo);
			await UniTask.WaitForSeconds(1.2f, cancellationToken: showScoreCts.Token);
			scoreText.text = score.ToString();
			await UniTask.WaitForSeconds(1.2f, cancellationToken: showScoreCts.Token);
		}

		public void Navigate(Vector2 value)
		{
		}

		public void Submit(bool value)
		{
			TryChangeScene();
		}

		public void Cancel(bool value)
		{
			TryChangeScene();
		}

		public void Config(bool value)
		{
			TryChangeScene();
		}

		public void Filter(bool value)
		{
			TryChangeScene();
		}

		public void View(bool value)
		{
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
