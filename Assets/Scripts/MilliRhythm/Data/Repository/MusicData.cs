using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.GameDataService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MilliRhythm.Data.Repository
{
	[CreateAssetMenu(fileName = "MusicData", menuName = "MilliRhythm/Data/MusicData")]
	public class MusicData : ScriptableObject
	{
		[field: SerializeField] public int Id { get; private set; }

		//Non-localized Text
		[field: SerializeField] public string Name { get; private set; }
		[field: SerializeField] public string ArtistName { get; private set; }
		[field: SerializeField] public string DescriptionKey { get; private set; }

		//Resource
		[field: SerializeField] public Sprite JacketThumbnail { get; private set; }
		[field: SerializeField] public string JacketPath { get; private set; }
		[field: SerializeField] public AssetReferenceT<AudioClip> AudioClipReference { get; private set; }
		[field: SerializeField] public string PreviewAudioPath { get; private set; }
	}

	public class MusicDataRepository : IGameDataRepository
	{
		private AsyncOperationHandle<MusicDataScriptableObject> handle;
		private MusicDataScriptableObject musicDataObject;
		private const string FileKey = "MusicDataRepository";

		private Dictionary<int, MusicData> musicDataMap = new();
		private List<MusicData> musicDataList;

		public async UniTask LoadAsync()
		{
			handle = Addressables.LoadAssetAsync<MusicDataScriptableObject>(FileKey);
			musicDataObject = await handle.Task;
			musicDataList = new List<MusicData>(musicDataObject.MusicDataList);
			foreach (var musicData in musicDataList)
			{
				musicDataMap.Add(musicData.Id, musicData);
			}
		}

		public void Release()
		{
			if (handle.IsValid())
				Addressables.Release(handle);
		}

		public List<MusicData> GetAllMusicData() => musicDataList;

		public MusicData GetMusicDataById(int id)
		{
			if (!musicDataMap.TryGetValue(id, out var data))
			{
				throw new KeyNotFoundException($"MusicData with id {id} not found");
			}

			return data;
		}

		public List<MusicData> GetMusicDataList() => musicDataList;
	}
}
