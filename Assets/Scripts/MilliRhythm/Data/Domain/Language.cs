using System;

namespace MilliRhythm.Data.Domain
{
	public enum LanguageType
	{
		Japanese,
		Korean,
		English,
	}

	public static class LanguageTypeExtensions
	{
		public static int ToInt(this LanguageType language)
		{
			return language switch
			{
				LanguageType.Japanese => 0,
				LanguageType.Korean => 1,
				LanguageType.English => 2,
				_ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
			};
		}

		public static LanguageType ToLanguageType(int language)
		{
			return language switch
			{
				0 => LanguageType.Japanese,
				1 => LanguageType.Korean,
				2 => LanguageType.English,
				_ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
			};
		}
	}
}
