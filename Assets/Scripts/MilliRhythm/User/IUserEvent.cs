using MilliRhythm.Data.Domain;

namespace MilliRhythm.User
{
	public struct Request
	{
		public IUserEvent evt;

		private Request(IUserEvent evt)
		{
			this.evt = evt;
		}

		public static Request Create(IUserEvent evt)
		{
			return new Request(evt);
		}
	}

	public class Response
	{
		public Result Result;
	}

	public interface IUserEvent
	{
	}

	public class SetScoreEvent : IUserEvent
	{
		public int TrackId;
		public ChartType ChartType;
		public int Score;
		public int Combo;
		public int PerfectCount;
		public int GreatCount;
		public int GoodCount;
		public int BadCount;
		public int MissCount;
	}
}
