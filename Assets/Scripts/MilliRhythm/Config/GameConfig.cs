using MilliRhythm.Data.Domain;

namespace MilliRhythm.Config
{
	public class GameConfig
	{
		public float MasterVolume { get; internal set; }
		public float MusicVolume { get; internal set; }
		public float SfxVolume { get; internal set; }

		public LanguageType Language;
		public KeyLayout KeyLayout;
	}

	public enum KeyLayout
	{
		WASD,
		SDKL,
	}
}
