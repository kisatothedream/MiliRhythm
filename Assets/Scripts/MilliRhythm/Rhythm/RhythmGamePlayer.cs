using UnityEngine;

namespace MilliRhythm.Rhythm
{
	public class RhythmGamePlayer : MonoBehaviour
	{
		private void Awake()
		{
			var startTime = AudioSettings.dspTime;
		}
	}
}
