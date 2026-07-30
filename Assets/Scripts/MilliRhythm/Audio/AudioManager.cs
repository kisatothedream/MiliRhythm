using MilliRhythm.Config;
using UnityEngine;
using UnityEngine.Audio;

namespace MilliRhythm.Audio
{
	public class AudioManager : MonoBehaviour
	{
		[SerializeField] private AudioMixer audioMixer;

		private const string MasterVolumeParameter = "MasterVolume";
		private const string MusicVolumeParameter = "MusicVolume";
		private const string SfxVolumeParameter = "SfxVolume";

		private void Awake()
		{
			ApplyConfig();
			ConfigManager.Instance.OnMasterVolumeChangedAction += OnMasterVolumeChanged;
			ConfigManager.Instance.OnMusicVolumeChangedAction += OnMusicVolumeChanged;
			ConfigManager.Instance.OnSfxVolumeChangedAction += OnSfxVolumeChanged;
		}

		private void OnDestroy()
		{
			ConfigManager.Instance.OnMasterVolumeChangedAction -= OnMasterVolumeChanged;
			ConfigManager.Instance.OnMusicVolumeChangedAction -= OnMusicVolumeChanged;
			ConfigManager.Instance.OnSfxVolumeChangedAction -= OnSfxVolumeChanged;
		}

		public void ApplyConfig()
		{
			var config = ConfigManager.Instance.Config;
			OnMasterVolumeChanged(config.MasterVolume);
			OnMusicVolumeChanged(config.MusicVolume);
			OnSfxVolumeChanged(config.SfxVolume);
		}

		private void OnMasterVolumeChanged(float volume) => audioMixer.SetFloat(MasterVolumeParameter, volume);
		private void OnMusicVolumeChanged(float volume) => audioMixer.SetFloat(MusicVolumeParameter, volume);
		private void OnSfxVolumeChanged(float volume) => audioMixer.SetFloat(SfxVolumeParameter, volume);
	}
}
