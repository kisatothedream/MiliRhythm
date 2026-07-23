using System.Collections.Generic;
using UnityEngine;

namespace MilliRhythm.Data.Repository
{
	public class MusicDataScriptableObject : ScriptableObject
	{
		public List<MusicData> MusicDataList => musicDataList;
		[SerializeField] private List<MusicData> musicDataList;

		public void Sort()
		{
			musicDataList.Sort((a, b) => a.Id.CompareTo(b.Id));
		}

		public void RegisterAllMusicData(List<MusicData> data)
		{
			musicDataList = new List<MusicData>(data);
		}
	}
}
