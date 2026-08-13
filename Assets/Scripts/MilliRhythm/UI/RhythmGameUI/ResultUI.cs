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
	public struct ResultUIParameter
	{
		public int Perfect;
		public int Great;
		public int Good;
		public int Bad;
		public int Miss;
		public int MaxCombo;
		public int Score;
	}

	public class ResultUI : MonoBehaviour, IUIInputListener
	{
		[SerializeField] private Button dismissArea;
		[SerializeField] private JudgeCountItem[] items;
		private bool closable;
		private CancellationTokenSource showScoreCts = new();

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
			showScoreCts = null;
			dismissArea.onClick.RemoveListener(TryChangeScene);
		}

		public async UniTask ShowResultAsync(ResultUIParameter resultUIParameter)
		{
			gameObject.SetActive(true);
			items[0].SetCountAndShow(resultUIParameter.Perfect);
			await UniTask.WaitForSeconds(0.2f, cancellationToken: showScoreCts.Token).SuppressCancellationThrow();
			items[1].SetCountAndShow(resultUIParameter.Great);
			await UniTask.WaitForSeconds(0.2f, cancellationToken: showScoreCts.Token).SuppressCancellationThrow();
			items[2].SetCountAndShow(resultUIParameter.Good);
			await UniTask.WaitForSeconds(0.2f, cancellationToken: showScoreCts.Token).SuppressCancellationThrow();
			items[3].SetCountAndShow(resultUIParameter.Bad);
			await UniTask.WaitForSeconds(0.2f, cancellationToken: showScoreCts.Token).SuppressCancellationThrow();
			items[4].SetCountAndShow(resultUIParameter.Miss);
			await UniTask.WaitForSeconds(0.2f, cancellationToken: showScoreCts.Token).SuppressCancellationThrow();
			items[5].SetCountAndShow(resultUIParameter.MaxCombo);
			await UniTask.WaitForSeconds(0.2f, cancellationToken: showScoreCts.Token).SuppressCancellationThrow();
			items[6].SetCountAndShow(resultUIParameter.Score);

			showScoreCts?.Cancel();
			showScoreCts?.Dispose();
			showScoreCts = null;
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
		}

		public void Config(bool value)
		{
		}

		public void Filter(bool value)
		{
		}

		public void AnyKey(bool value)
		{
		}

		private void TryChangeScene()
		{
			if (!closable)
			{
				closable = true;
			}
			else
			{
				SceneController.Instance.RequestChangeScene(new RhythmGameSceneParameter());
			}
		}
	}
}
