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
	}
}
