using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MilliRhythm.Audio
{
	public sealed class SfxAudioPlayer : MonoBehaviour
	{
		public static SfxAudioPlayer Instance { get; private set; }
		[SerializeField] private SfxRepository sfxRepository;

		[SerializeField] private AudioSource audioSource;

		private Dictionary<SfxType, AudioClip> clips;

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);

			clips = sfxRepository.SfxLists.ToDictionary(x => x.SfxType, x => x.Clip);
		}

		public void Play(SfxType type)
		{
			if (clips.TryGetValue(type, out var clip))
				audioSource.PlayOneShot(clip);
		}
	}
}
