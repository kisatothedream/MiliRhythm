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
		private const string KeyLayoutKey = "MilliRhythm.Config.KeyLayout";

		public GameConfig Config { get; private set; }

		public event Action<float> OnMasterVolumeChangedAction;
		public event Action<float> OnMusicVolumeChangedAction;
		public event Action<float> OnSfxVolumeChangedAction;
		public event Action<LanguageType> OnLanguageChangedAction;
		public event Action<KeyLayout> OnKeyLayoutChangedAction;

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
				KeyLayout = (KeyLayout)PlayerPrefs.GetInt(KeyLayoutKey, 0),
			};
			ApplyAllConfigs(Config);
		}

		private void ApplyAllConfigs(GameConfig config)
		{
			ChangeMasterVolume(config.MasterVolume);
			ChangeMusicVolume(config.MusicVolume);
			ChangeSfxVolume(config.SfxVolume);
			ChangeKeyLayout(config.KeyLayout);
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

		public void ChangeKeyLayout(KeyLayout layout)
		{
			Config.KeyLayout = layout;
			OnKeyLayoutChangedAction?.Invoke(Config.KeyLayout);
		}

		private float ConvertVolumeToDb(float volume)
		{
			if (volume <= 0)
			{
				return -80f;
			}

			return 20 * Mathf.Log10(volume);
		}

		public void ChangeLanguage(LanguageType type)
		{
			Config.Language = type;
			OnLanguageChangedAction?.Invoke(Config.Language);
		}

		public void Save()
		{
			PlayerPrefs.SetFloat(MasterVolumeKey, Config.MasterVolume);
			PlayerPrefs.SetFloat(MusicVolumeKey, Config.MusicVolume);
			PlayerPrefs.SetFloat(SfxVolumeKey, Config.SfxVolume);
			PlayerPrefs.SetInt(LanguageKey, Config.Language.ToInt());
			PlayerPrefs.SetInt(KeyLayoutKey, (int)Config.KeyLayout);
			PlayerPrefs.Save();
		}
	}
}
