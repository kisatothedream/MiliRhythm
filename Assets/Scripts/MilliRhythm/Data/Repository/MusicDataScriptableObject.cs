using System.Collections.Generic;
using UnityEngine;

namespace MilliRhythm.Data.Repository
{
	public class MusicDataScriptableObject : ScriptableObject
	{
		public List<MusicData> MusicDataList => musicDataList;
		[SerializeField] private List<MusicData> musicDataList;
	}
}
