using System.Collections.Generic;
using MilliRhythm.Rhythm;
using UnityEngine;

namespace MilliRhythm.Data.Repository
{
	public class RhythmChartDataScriptableObject : ScriptableObject
	{
		public List<RhythmChart> ChartDataList => chartDataList;
		[SerializeField] private List<RhythmChart> chartDataList;

		public void RegisterAllChartData(List<RhythmChart> data)
		{
			chartDataList = new List<RhythmChart>(data);
		}

		public void Sort()
		{
			chartDataList.Sort((a, b) => a.MusicId.CompareTo(b.MusicId));
		}
	}
}
