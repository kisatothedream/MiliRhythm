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

		private void Start()
		{
			DontDestroyOnLoad(gameObject);

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

		private void ApplyConfig()
		{
			var config = ConfigManager.Instance.Config;
			OnMasterVolumeChanged(ConvertVolumeToDb(config.MasterVolume));
			OnMusicVolumeChanged(ConvertVolumeToDb(config.MusicVolume));
			OnSfxVolumeChanged(ConvertVolumeToDb(config.SfxVolume));
		}

		private void OnMasterVolumeChanged(float volume) => audioMixer.SetFloat(MasterVolumeParameter, volume);
		private void OnMusicVolumeChanged(float volume) => audioMixer.SetFloat(MusicVolumeParameter, volume);
		private void OnSfxVolumeChanged(float volume) => audioMixer.SetFloat(SfxVolumeParameter, volume);

		private float ConvertVolumeToDb(float volume)
		{
			if (volume <= 0)
			{
				return -80f;
			}

			return 20 * Mathf.Log10(volume);
		}
	}
}
