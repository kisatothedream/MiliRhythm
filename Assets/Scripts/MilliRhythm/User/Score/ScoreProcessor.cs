using Cysharp.Threading.Tasks;

namespace MilliRhythm.User.Score
{
	public class ScoreProcessor : ISubProcessor
	{
		private ScoreDataModel model => UserManager.Model.ScoreDataModel;

		public async UniTask SetGameInitialData()
		{
		}

		public async UniTask Init()
		{
		}

		public bool Process(IUserEvent evt)
		{
			switch (evt)
			{
				case SetScoreEvent scoreEvent:
					SetScore(scoreEvent);
					break;
			}

			return true;
		}

		private void SetScore(SetScoreEvent evt)
		{
			if (!model.ScoreDataMap.TryGetValue((evt.TrackId, evt.ChartType), out var scoreData))
			{
				scoreData = new ScoreData()
				{
					MusicId = evt.TrackId,
					Score = evt.Score,
					Combo = evt.Combo,
					PerfectCount = evt.PerfectCount,
					GreatCount = evt.GreatCount,
					GoodCount = evt.GoodCount,
					BadCount = evt.BadCount,
					MissCount = evt.MissCount,
				};
				model.ScoreDataMap.Add((evt.TrackId, evt.ChartType), scoreData);
			}

			UserManager.CommandSetScore(evt.TrackId, evt.ChartType, evt.Score, evt.Combo, evt.PerfectCount, evt.GreatCount, evt.GoodCount, evt.BadCount, evt.MissCount);
		}
	}
}
