using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.GameDataService;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
		[field: SerializeField] public string AudioPath { get; private set; }
		[field: SerializeField] public string PreviewAudioPath { get; private set; }
	}

	public class MusicDataRepository : IGameDataRepository
	{
		private MusicDataScriptableObject musicDataObject;
		private const string FileName = "Assets/Data/Music/MusicDataRepository.asset";

		private Dictionary<int, MusicData> musicDataMap = new();
		private List<MusicData> musicDataList;

		public async UniTask LoadAsync()
		{
			musicDataObject = await Addressables.LoadAssetAsync<MusicDataScriptableObject>(FileName).Task;
			musicDataList = new List<MusicData>(musicDataObject.MusicDataList);
			foreach (var musicData in musicDataList)
			{
				musicDataMap.Add(musicData.Id, musicData);
			}
		}

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
