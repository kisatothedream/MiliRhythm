using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MilliRhythm.Data.Repository
{
	public class MusicDataScriptableObject : ScriptableObject
	{
		[SerializeField] private List<MusicData> MusicDataList = new List<MusicData>();
		public Dictionary<int, MusicData> GetMusicDataMap() => MusicDataList.ToDictionary(k => k.Id, v => v);
	}
}
