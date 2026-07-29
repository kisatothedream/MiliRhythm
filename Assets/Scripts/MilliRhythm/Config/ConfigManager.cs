using MilliRhythm.Data.Repository;
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
				Language = LanguageTypeExtensions.ToLanguageType(PlayerPrefs.GetInt(LanguageKey, 0)),
			};
		}

	}
}
