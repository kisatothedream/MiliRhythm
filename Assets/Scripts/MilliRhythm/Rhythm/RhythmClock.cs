using System;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	[Serializable]
	public class RhythmClock
	{
		private double songStartDspTime;
		private bool isRunning;

		public double SongTime
		{
			get
			{
				if (!isRunning) return 0;
				return AudioSettings.dspTime - songStartDspTime;
			}
		}

		public void StartClock(double startDspTime)
		{
			songStartDspTime = startDspTime;
			isRunning = true;
		}

		public void StopClock()
		{
			isRunning = false;
		}
	}
}
