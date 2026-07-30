using UnityEngine;

namespace MilliRhythm.Audio
{
	public class TrackSelectorAudioPlayer : MonoBehaviour
	{
		[SerializeField] private AudioSource trackAudioSource;

		public void PlayTrackPreview(AudioClip clip)
		{
			trackAudioSource.clip = clip;
			trackAudioSource.Play();
		}

		public void StopTrackPreview()
		{
			trackAudioSource.Stop();
		}
	}
}
