using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.GameDataService;
using MilliRhythm.TrackSelector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MilliRhythm.UI.TrackSelectorUI
{
	public class TrackSelectorCanvas : MonoBehaviour
	{
		[SerializeField] private TrackSelectorList trackSelectorList = new();

		//Track Detail Info Panel
		[SerializeField] private Sprite jacketPlaceHolderSprite;

		private CancellationTokenSource trackLoadingCts;
		private AsyncOperationHandle<Sprite>? currentJacketHandle;

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
			}

			trackSelectorList.Set(models);
			trackSelectorList.OnSelectionChanged = OnTrackSelectionChanged;
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

			try
			{
				await UniTask.WaitForSeconds(0.2f, cancellationToken: trackLoadingCts.Token);
				var jacketHandle = await LoadJacketAsync(model.JacketSpriteReference, trackLoadingCts.Token);
				ReleaseCurrentJacket();
				currentJacketHandle = jacketHandle;
				
				var jacketSprite = jacketHandle.Result;
				
				var previewAudioClip = model.PreviewAudioClip;
			}
			catch (OperationCanceledException e)
			{
			}
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
	}
}
