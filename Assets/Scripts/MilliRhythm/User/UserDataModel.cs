using System.Collections.Generic;
using System.Linq;
using MilliRhythm.User.Save;
using MilliRhythm.User.Score;

namespace MilliRhythm.User
{
	public class UserDataModel
	{
		public readonly ScoreDataModel ScoreDataModel = new();

		internal void ApplySave(UserSaveData saveData)
		{
			ScoreDataModel.ApplySave(saveData);
		}

		internal UserSaveData ToSaveData()
		{
			var save = new UserSaveData
			{
				Scores = new List<ScoreData>(ScoreDataModel.ScoreDataMap.Select(item => item.Value)),
			};

			return save;
		}
	}

	public interface IUserData
	{
		public void ApplySave(UserSaveData saveData);
	}
}
