using System;
using System.Collections.Generic;
using MilliRhythm.User.Score;

namespace MilliRhythm.User.Save
{
	[Serializable]
	public class UserSaveData
	{
		public List<ScoreData> Scores = new();
	}
}
