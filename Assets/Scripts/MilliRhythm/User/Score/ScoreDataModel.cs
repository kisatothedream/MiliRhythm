using System;
using System.Collections.Generic;
using MilliRhythm.Data.Domain;
using MilliRhythm.User.Save;

namespace MilliRhythm.User.Score
{
	public class ScoreDataModel : IUserData
	{
		internal Dictionary<string, ScoreData> ScoreDataMap = new();

		public void ApplySave(UserSaveData saveData)
		{
			ScoreDataMap.Clear();
			foreach (var score in saveData.Scores)
			{
				ScoreDataMap.Add($"{score.MusicId}_{score.ChartType}", score);
			}
		}

		public bool TryGetScoreData(int id, ChartType type, out ScoreData data)
		{
			return TryGetScoreData((id, type), out data);
		}

		public bool TryGetScoreData((int id, ChartType type) tuple, out ScoreData data)
		{
			return ScoreDataMap.TryGetValue($"{tuple.id}_{tuple.type}", out data);
		}
	}

	[Serializable]
	public class ScoreData
	{
		public int MusicId;
		public ChartType ChartType;
		public int Combo;
		public int Score;
		public int PerfectCount;
		public int GreatCount;
		public int GoodCount;
		public int BadCount;
		public int MissCount;

		public ScoreData()
		{
		}
	}
}
