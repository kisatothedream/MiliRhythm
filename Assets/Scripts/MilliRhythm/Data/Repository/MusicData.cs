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
		public int Id;

		//Non-localized Text
		public string Name;
		public string ArtistName;
		public string DescriptionKey;

		//Resource
		public string AudioPath;
		public string JacketPath;
	}

	public class MusicDataRepository : IGameDataRepository
	{
		private MusicDataScriptableObject musicDataObject;
		private const string FileName = "Assets/Data/Music/MusicDataRepository.asset";

		private Dictionary<int, MusicData> musicDataMap = new Dictionary<int, MusicData>();

		public async UniTask LoadAsync()
		{
			musicDataObject = await Addressables.LoadAssetAsync<MusicDataScriptableObject>(FileName).Task;
			musicDataMap = musicDataObject.GetMusicDataMap();
		}

		public MusicData GetMusicDataById(int id)
		{
			if (!musicDataMap.TryGetValue(id, out var data))
			{
				throw new KeyNotFoundException($"MusicData with id {id} not found");
			}

			return data;
		}
	}
}
