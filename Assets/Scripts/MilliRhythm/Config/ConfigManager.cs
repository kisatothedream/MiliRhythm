using System;
using MilliRhythm.Data.Domain;
using UnityEngine;

namespace MilliRhythm.Config
{
	public class ConfigManager
	{
		public static ConfigManager Instance => instance ??= new ConfigManager();
		private static ConfigManager instance;
		private const string MasterVolumeKey = "MilliRhythm.Config.MasterVolume";
		private const string MusicVolumeKey = "MilliRhythm.Config.MusicVolume";
		private const string SfxVolumeKey = "MilliRhythm.Config.SfxVolume";
		private const string LanguageKey = "MilliRhythm.Config.Language";

		public GameConfig Config { get; private set; }

		public event Action<float> OnMasterVolumeChangedAction;
		public event Action<float> OnMusicVolumeChangedAction;
		public event Action<float> OnSfxVolumeChangedAction;

		private ConfigManager()
		{
		}

		public void Load()
		{
			Config = new GameConfig
			{
				MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 0.5f),
				MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f),
				SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0.5f),
				Language = LanguageTypeExtensions.ToLanguageType(PlayerPrefs.GetInt(LanguageKey, LanguageType.Japanese.ToInt())),
			};
			ApplyAllConfigs(Config);
		}

		private void ApplyAllConfigs(GameConfig config)
		{
			ChangeMasterVolume(config.MasterVolume);
			ChangeMusicVolume(config.MusicVolume);
			ChangeSfxVolume(config.SfxVolume);
		}

		public void ChangeMasterVolume(float volume)
		{
			Config.MasterVolume = volume;
			OnMasterVolumeChangedAction?.Invoke(ConvertVolumeToDb(Config.MasterVolume));
		}

		public void ChangeMusicVolume(float volume)
		{
			Config.MusicVolume = volume;
			OnMusicVolumeChangedAction?.Invoke(ConvertVolumeToDb(Config.MusicVolume));
		}

		public void ChangeSfxVolume(float volume)
		{
			Config.SfxVolume = volume;
			OnSfxVolumeChangedAction?.Invoke(ConvertVolumeToDb(Config.SfxVolume));
		}

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
