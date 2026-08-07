using MilliRhythm.Data.Domain;
using MilliRhythm.Rhythm;

namespace MilliRhythm.Scene.Contracts
{
	public interface IGameSceneParameter
	{
	}

	public class RhythmGameSceneParameter : IGameSceneParameter
	{
		public int MusicId;
		public ChartType ChartType;
		public Difficulty Difficulty;

		public RhythmGameSceneParameter(int musicId, ChartType chartType, Difficulty difficulty)
		{
			MusicId = musicId;
			ChartType = chartType;
			Difficulty = difficulty;
		}
	}

	public class MusicSelectorSceneParameter : IGameSceneParameter
	{
		public int LastMusicId;
		public ChartType LastChartType;
		public Difficulty LastDifficulty;

		public MusicSelectorSceneParameter()
		{
			LastMusicId = -1;
		}

		public MusicSelectorSceneParameter(int lastMusicId, ChartType lastChartType, Difficulty lastDifficulty)
		{
			LastMusicId = lastMusicId;
			LastChartType = lastChartType;
			LastDifficulty = lastDifficulty;
		}
	}
}
