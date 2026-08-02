using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MilliRhythm.Audio;
using MilliRhythm.Data.Domain;
using MilliRhythm.Data.GameDataService;
using MilliRhythm.UI.Components;
using MilliRhythm.UI.ConfigUI;
using MilliRhythm.UI.TrackSelectorUI.Filter;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace MilliRhythm.UI.TrackSelectorUI
{
	public class TrackSelectorUIController : MonoBehaviour
	{
		[SerializeField] private TrackSelectorListPanel trackSelectorPanel;
		[SerializeField] private TrackInfoPanel trackInfoPanel;
		[SerializeField] private VocalFilterPopup filterPopup;
		[SerializeField] private Button filterButton;
		[SerializeField] private ConfigPanel configCanvas;
		[SerializeField] private Button configButton;

		//Track Detail Info Panel
		[SerializeField] private Sprite jacketPlaceHolderSprite;

		private CancellationTokenSource trackLoadingCts;
		private AsyncOperationHandle<Sprite>? currentJacketHandle;

		[SerializeField] private TrackSelectorAudioPlayer audioPlayer;

		private void Awake()
		{
			filterButton.onClick.AddListener(DisplayFilterPopup);
			configButton.onClick.AddListener(DisplayConfigPopup);
		}

		private void OnDestroy()
		{
			filterButton.onClick.RemoveListener(DisplayFilterPopup);
			configButton.onClick.RemoveListener(DisplayConfigPopup);
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
					TrackName = musicData.Name,
					TrackVocal = musicData.Vocals,
				};
				models.Add(model);
			}

			trackSelectorPanel.Set(models, OnTrackSelectionChanged);
		}

		public void Finish()
		{
			trackSelectorPanel.Finish();
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
			// trackLoadingCts?.Cancel();
			// trackLoadingCts?.Dispose();
			// trackLoadingCts = new CancellationTokenSource();

			// try
			// {
			trackInfoPanel.ResetToPlaceholder();
			audioPlayer.StopTrackPreview();
			// await UniTask.WaitForSeconds(0.2f, cancellationToken: trackLoadingCts.Token);
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
			trackInfoPanel.SetTrackInfo(jacketSprite, model.TrackName, "");
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
			DisplayFilterPopupAsync().Forget();
			return;

			async UniTask DisplayFilterPopupAsync()
			{
				var result = await filterPopup.Display(new VocalFilterPopupParameter()
				{
					LastFilter = Member.AkubiDemonspade,
				});
				if (result.Result == PopupResult.Confirm)
				{
					var filter = result.Payload.FilterMember;
					trackSelectorPanel.ApplyFilter(filter);
				}
			}
		}

		public void DisplayConfigPopup()
		{
			configCanvas.Show();
		}
	}
}
