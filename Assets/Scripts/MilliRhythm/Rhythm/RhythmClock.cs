using System;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	[Serializable]
	public class RhythmClock
	{
		private double pauseStartDspTime;
		private double songStartDspTime;
		private bool isRunning;
		public bool IsPaused { get; private set; }

		public double SongTime
		{
			get
			{
				if (!isRunning) return 0;
				var currentDspTime = IsPaused
					? pauseStartDspTime
					: AudioSettings.dspTime;

				return currentDspTime - songStartDspTime;
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

		public void PauseClock()
		{
			pauseStartDspTime = AudioSettings.dspTime;
			IsPaused = true;
		}

		public void ResumeClock()
		{
			var pausedDuration = AudioSettings.dspTime - pauseStartDspTime;
			songStartDspTime += pausedDuration;
			IsPaused = false;
		}
	}
}
