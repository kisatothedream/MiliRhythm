using System;
using MilliRhythm.Config;
using UnityEngine;
using UnityEngine.Audio;

namespace MilliRhythm.Audio
{
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance => instance;
		private static AudioManager instance;

		[SerializeField] private AudioMixer audioMixer;

		private const string MasterVolumeParameter = "MasterVolume";
		private const string MusicVolumeParameter = "BgmVolume";
		private const string SfxVolumeParameter = "SfxVolume";

		private void Awake()
		{
			ApplyConfig();
		}

		public void ApplyConfig()
		{
			var config = ConfigManager.Instance.Config;
			audioMixer.SetFloat(MasterVolumeParameter, config.MasterVolume);
			audioMixer.SetFloat(MusicVolumeParameter, config.MusicVolume);
			audioMixer.SetFloat(SfxVolumeParameter, config.SfxVolume);
		}
	}
}
