using System;
using System.Collections.Generic;
using MilliRhythm.User.Save;

namespace MilliRhythm.User.Score
{
	public class ScoreDataModel : IUserData
	{
		public Dictionary<int, ScoreData> ScoreDataMap = new();

		public void ApplySave(UserSaveData saveData)
		{
			ScoreDataMap.Clear();
			foreach (var score in saveData.Scores)
			{
				ScoreDataMap.Add(score.TrackId, score);
			}
		}
	}

	[Serializable]
	public class ScoreData
	{
		public int TrackId;
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
