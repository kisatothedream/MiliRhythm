using MilliRhythm.Data.Repository;
using UnityEngine;

namespace MilliRhythm.Config
{
	public class GameConfig
	{
		public float MasterVolume;
		public float MusicVolume;
		public float SfxVolume;

		public LanguageType Language;


		public void ChangeMasterVolume(float volume)
		{
			MasterVolume = volume;
		}

		public void ChangeMusicVolume()
		{
			MusicVolume = MasterVolume;
		}

		public void ChangeSfxVolume()
		{
			SfxVolume = MasterVolume;
		}

		public float ConvertVolumeToDb(float volume)
		{
			if (volume <= 0)
			{
				return -80f;
			}

			return 20 * Mathf.Log10(volume);
		}
	}
}
