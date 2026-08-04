using Cysharp.Threading.Tasks;
using MilliRhythm.Data.Domain;
using MilliRhythm.User.Save;
using MilliRhythm.User.Score;
using UnityEngine;

namespace MilliRhythm.User
{
	public partial class UserManager
	{
		public static UserManager Instance
		{
			get
			{
				instance ??= new UserManager();
				return instance;
			}
		}

		private static UserManager instance;
		private static UserProcessor userProcessor;
		public static UserDataModel Model;


		private UserManager()
		{
		}

		public static async UniTask Init()
		{
			instance = new UserManager();
			Model = new UserDataModel();
			userProcessor = new UserProcessor();
			await userProcessor.Init();
			if (TryLoad(out var model))
			{
				Debug.Log("Load Success.");
				Model.ApplySave(model);
			}
			else
			{
				//없다면 최초로 게임 상태 적용
				await userProcessor.SetGameInitialState();
			}

			Debug.Log("UserManager Initialized.");
		}

		public static Response SendRequest(Request request)
		{
			return userProcessor.ReceiveRequest(request);
		}

		private static bool TryLoad(out UserSaveData saveData)
		{
			return SaveManager.TryLoad(out saveData);
		}

		public static void RequestSave()
		{
			Save();
		}

		private static void Save()
		{
			SaveManager.Save(Model.ToSaveData());
		}
	}

	public partial class UserManager
	{
		internal static void CommandSetScore(int musicId, ChartType type, int score, int combo, int perfect, int great, int good, int bad, int miss)
		{
			if (Model.ScoreDataModel.ScoreDataMap.TryGetValue((musicId, type), out var scoreData))
			{
				if (score > scoreData.Score)
				{
					scoreData.Score = score;
					scoreData.Combo = combo;
					scoreData.PerfectCount = perfect;
					scoreData.GreatCount = great;
					scoreData.GoodCount = good;
					scoreData.BadCount = bad;
					scoreData.MissCount = miss;
				}
				else
				{
					scoreData = new ScoreData()
					{
						MusicId = musicId,
						ChartType = type,
						Score = score,
						Combo = combo,
						PerfectCount = perfect,
						GreatCount = great,
						GoodCount = good,
						BadCount = bad,
						MissCount = miss
					};
				}

				Model.ScoreDataModel.ScoreDataMap[(musicId, type)] = scoreData;
			}
		}
	}

	public enum Result
	{
		Success,
		Failure,
	}
}
