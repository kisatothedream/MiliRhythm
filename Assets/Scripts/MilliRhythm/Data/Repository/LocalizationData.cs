using System;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.GameDataService;

namespace MilliRhythm.Data.Repository
{
	public sealed class LocalizationData : IGameData<string>
	{
		public string Id { get; set; }
		public string Text { get; set; }
	}

	public sealed class LocalizationDataRepository : CsvDataRepository<string, LocalizationData>
	{
		private readonly LanguageType language;
		private string fileName;
		protected override string FileName => fileName;

		public LocalizationDataRepository(LanguageType language)
		{
			this.language = language;
		}

		public override async UniTask LoadAsync()
		{
			fileName = GetFileName(language);
			await base.LoadAsync();
		}

		private static string GetFileName(LanguageType language)
		{
			return language switch
			{
				LanguageType.Korean => "Localization/Localization_ko.csv",
				LanguageType.English => "Localization/Localization_en.csv",
				LanguageType.Japanese => "Localization/Localization_jp.csv",
				_ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
			};
		}

		public string GetText(string key)
		{
			return Get(key).Text;
		}
	}

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
