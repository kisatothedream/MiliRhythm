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
		[SerializeField] private ControlDescPanel controlDescPanel;

		private Member appliedFilter;

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
			InputManager.Instance.OnChangeDeviceEvent += controlDescPanel.OnChangeDevice;
		}

		private void OnDestroy()
		{
			filterButton.onClick.RemoveListener(DisplayFilterPopup);
			configButton.onClick.RemoveListener(DisplayConfigPopup);
			startButton.onClick.RemoveListener(DisplayGameStartPopup);
			InputManager.Instance.OnChangeDeviceEvent -= controlDescPanel.OnChangeDevice;
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

		public void SelectLastTrack(int trackId)
		{
			trackSelectorPanel.SelectLastTrack(trackId);
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
			var hasMScore = UserManager.Model.ScoreDataModel.TryGetScoreData(model.Id, ChartType.Melody, out var mScoreData);
			var hasBScore = UserManager.Model.ScoreDataModel.TryGetScoreData(model.Id, ChartType.Beat, out var bScoreData);
			var mScore = hasMScore ? mScoreData.Score : 0;
			var bScore = hasBScore ? bScoreData.Score : 0;
			var mRank = hasMScore ? mScoreData.Rank : Rank.N;
			var bRank = hasBScore ? bScoreData.Rank : Rank.N;

			trackInfoPanel.SetTrackInfo(jacketSprite, model.TrackNameKey, GameDataService.GetMemberData(model.TrackVocal).NameKey, mScore, bScore, mRank,
				bRank);
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
					LastFilter = appliedFilter,
				});
				if (result.Result == PopupResult.Confirm)
				{
					var filter = result.Payload.FilterMember;
					appliedFilter = filter;
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
			SfxAudioPlayer.Instance.Play(SfxType.Cancel);
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
			SfxAudioPlayer.Instance.Play(SfxType.Cancel);
			DisplayGameStartPopupAsync().Forget();
			return;

			async UniTask DisplayGameStartPopupAsync()
			{
				isPopupOpened = true;
				var model = trackSelectorPanel.SelectedTrackModel;
				var hasMScore = UserManager.Model.ScoreDataModel.TryGetScoreData(model.Id, ChartType.Melody, out var mScoreData);
				var hasBScore = UserManager.Model.ScoreDataModel.TryGetScoreData(model.Id, ChartType.Beat, out var bScoreData);
				var result = await startPopup.Display(new GameStartPopupParameter()
				{
					Model = model,
					MelodyScore = hasMScore ? mScoreData.Score : 0,
					BeatScore = hasBScore ? bScoreData.Score : 0,
					MelodyRank = hasMScore ? mScoreData.Rank : Rank.N,
					BeatRank = hasBScore ? bScoreData.Rank : Rank.N,
				});
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
			SfxAudioPlayer.Instance.Play(SfxType.Navigate);
		}

		public void Submit(bool value)
		{
			if (isPopupOpened) return;
			if (!value) return;
			DisplayGameStartPopup();
			SfxAudioPlayer.Instance.Play(SfxType.Confirm);
		}

		public void Cancel(bool value)
		{
		}

		public void Config(bool value)
		{
			if (isPopupOpened) return;
			if (!value) return;
			DisplayConfigPopup();
		}

		public void Filter(bool value)
		{
			if (isPopupOpened) return;
			if (!value) return;
			DisplayFilterPopup();
		}
	}
}
