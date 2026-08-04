using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MilliRhythm.Audio;
using MilliRhythm.Data.Domain;
using MilliRhythm.Data.GameDataService;
using MilliRhythm.Input;
using MilliRhythm.Scene;
using MilliRhythm.Scene.Contracts;
using MilliRhythm.UI.Components;
using MilliRhythm.UI.ConfigUI;
using MilliRhythm.UI.TrackSelectorUI.Filter;
using MilliRhythm.UI.TrackSelectorUI.StartPopup;
using MilliRhythm.User;
using MilliRhythm.User.Score;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace MilliRhythm.UI.TrackSelectorUI
{
	public class TrackSelectorUIController : MonoBehaviour, IUIInputListener
	{
		[SerializeField] private TrackSelectorListPanel trackSelectorPanel;
		[SerializeField] private TrackInfoPanel trackInfoPanel;
		[SerializeField] private VocalFilterPopup filterPopup;
		[SerializeField] private Button filterButton;
		[SerializeField] private ConfigPopup configPopup;
		[SerializeField] private Button configButton;
		[SerializeField] private GameStartPopup startPopup;
		[SerializeField] private Button startButton;

		//Track Detail Info Panel
		[SerializeField] private Sprite jacketPlaceHolderSprite;

		private CancellationTokenSource trackLoadingCts;
		private AsyncOperationHandle<Sprite>? currentJacketHandle;

		[SerializeField] private TrackSelectorAudioPlayer audioPlayer;

		private bool isPopupOpened;

		private void Awake()
		{
			filterButton.onClick.AddListener(DisplayFilterPopup);
			configButton.onClick.AddListener(DisplayConfigPopup);
			startButton.onClick.AddListener(DisplayGameStartPopup);
		}

		private void OnDestroy()
		{
			filterButton.onClick.RemoveListener(DisplayFilterPopup);
			configButton.onClick.RemoveListener(DisplayConfigPopup);
			startButton.onClick.RemoveListener(DisplayGameStartPopup);
		}

		public void Set()
		{
			var data = GameDataService.GetAllMusicData();
			var models = new List<TrackSelectorListModel>();
			foreach (var musicData in data)
			{
				var model = new TrackSelectorListModel(musicData.Id)
				{
					ThumbnailSprite = musicData.ThumbnailSprite,
					JacketSpriteReference = musicData.JacketSprite,
					PreviewAudioClip = musicData.PreviewAudioClip,
					AudioClipReference = musicData.AudioClipReference,
					TrackNameKey = musicData.NameKey,
					TrackVocal = musicData.Vocals,
				};
				models.Add(model);
			}

			trackSelectorPanel.Set(models, OnTrackSelectionChanged);
			this.RegisterUIInputListener();
		}

		public void Finish()
		{
			trackSelectorPanel.Finish();
			this.UnregisterUIInputListener();
		}

		private void OnTrackSelectionChanged(TrackSelectorListModel model)
		{
			//Track Detail Info Panel에 정보 출력
			//1초 후 이미지 로딩 + 음악 프리뷰 재생
			//아래 정보 하나씩 등장하는 연출?
			SelectTrackAsync(model).Forget();
		}

		private async UniTask SelectTrackAsync(TrackSelectorListModel model)
		{
			trackLoadingCts?.Cancel();
			trackLoadingCts?.Dispose();
			trackLoadingCts = new CancellationTokenSource();

			// try
			// {
			trackInfoPanel.ResetToPlaceholder();
			audioPlayer.StopTrackPreview();
			await UniTask.WaitForSeconds(0.2f, cancellationToken: trackLoadingCts.Token);
			// var jacketHandle = await LoadJacketAsync(model.JacketSpriteReference, trackLoadingCts.Token);
			// ReleaseCurrentJacket();
			// currentJacketHandle = jacketHandle;

			// var jacketSprite = jacketHandle.Result;

			if (model == null)
			{
				//TODO : 필터 결과가 없다는 것 보여주기
				return;
			}

			var jacketSprite = model.ThumbnailSprite;
			var mScore = UserManager.Model.ScoreDataModel.TryGetScoreData(model.Id, ChartType.Melody, out var mScoreData) ? mScoreData.Score : 0;
			var bScore = UserManager.Model.ScoreDataModel.TryGetScoreData(model.Id, ChartType.Beat, out var bScoreData) ? bScoreData.Score : 0;

			trackInfoPanel.SetTrackInfo(jacketSprite, model.TrackNameKey, GameDataService.GetMemberData(model.TrackVocal).NameKey, mScore, bScore);
			var previewAudioClip = model.PreviewAudioClip;
			audioPlayer.PlayTrackPreview(previewAudioClip);
			// }
			// catch (OperationCanceledException e)
			// {
			// }
		}

		private async UniTask<AsyncOperationHandle<Sprite>> LoadJacketAsync(AssetReferenceSprite spriteReference, CancellationToken token)
		{
			var handle = spriteReference.LoadAssetAsync();
			try
			{
				await handle.Task.AsUniTask().AttachExternalCancellation(token);
				return handle;
			}
			catch
			{
				if (handle.IsValid())
					Addressables.Release(handle);
				throw;
			}
		}

		private void ReleaseCurrentJacket()
		{
			if (!currentJacketHandle.HasValue)
				return;

			var handle = currentJacketHandle.Value;

			if (handle.IsValid())
				Addressables.Release(handle);

			currentJacketHandle = null;
		}

		public void DisplayFilterPopup()
		{
			if (isPopupOpened) return;
			DisplayFilterPopupAsync().Forget();
			return;

			async UniTask DisplayFilterPopupAsync()
			{
				isPopupOpened = true;
				var result = await filterPopup.Display(new VocalFilterPopupParameter()
				{
					LastFilter = Member.AkubiDemonspade,
				});
				if (result.Result == PopupResult.Confirm)
				{
					var filter = result.Payload.FilterMember;
					UpdateFilterButton(filter == 0);
					trackSelectorPanel.ApplyFilter(filter);
				}

				isPopupOpened = false;
			}
		}

		private void UpdateFilterButton(bool hasFilter)
		{
			//TODO: Apply Filtered Image
		}

		private void DisplayConfigPopup()
		{
			if (isPopupOpened) return;
			DisplayConfigPopupAsync().Forget();
			return;

			async UniTask DisplayConfigPopupAsync()
			{
				isPopupOpened = true;
				await configPopup.Display(null);
				isPopupOpened = false;
			}
		}

		private void DisplayGameStartPopup()
		{
			if (isPopupOpened) return;
			if (trackSelectorPanel.SelectedTrackModel == null) return;
			DisplayGameStartPopupAsync().Forget();
			return;

			async UniTask DisplayGameStartPopupAsync()
			{
				isPopupOpened = true;
				var result = await startPopup.Display(new GameStartPopupParameter() { Model = trackSelectorPanel.SelectedTrackModel });
				isPopupOpened = false;
				if (result.Result == PopupResult.Confirm)
				{
					var payload = result.Payload;
					SceneController.Instance.RequestChangeScene(new RhythmGameSceneParameter(payload.TrackId, payload.ChartType, payload.Difficulty));
				}
			}
		}

		public void Navigate(Vector2 value)
		{
			if (isPopupOpened) return;
			trackSelectorPanel.OnNavigate(value);
		}

		public void Submit(bool value)
		{
			if (isPopupOpened) return;
			if (!value) return;
			DisplayGameStartPopup();
		}

		public void Cancel(bool value)
		{
			if (isPopupOpened) return;
			if (!value) return;
			DisplayConfigPopup();
		}

		public void View(bool value)
		{
			if (isPopupOpened) return;
			if (!value) return;
			DisplayFilterPopup();
		}
	}
}
