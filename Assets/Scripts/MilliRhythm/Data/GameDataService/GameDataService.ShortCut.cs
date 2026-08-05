using System.Collections.Generic;
using MilliRhythm.Data.Domain;
using MilliRhythm.Data.Repository;
using MilliRhythm.Rhythm;

namespace MilliRhythm.Data.GameDataService
{
	public static partial class GameDataService
	{
		public static ItemData GetItem(int itemId) => ((ItemDataRepository)repositories[typeof(ItemDataRepository)]).Get(itemId);
		public static string GetText(string key) => ((LocalizationDataRepository)repositories[typeof(LocalizationDataRepository)]).GetText(key);
		public static List<MusicData> GetAllMusicData() => ((MusicDataRepository)repositories[typeof(MusicDataRepository)]).GetAllMusicData();
		public static MusicData GetMusicData(int id) => ((MusicDataRepository)repositories[typeof(MusicDataRepository)]).GetMusicDataById(id);

		public static RhythmChart GetChartData(int id, ChartType chartType, Difficulty difficulty) =>
			((RhythmChartRepository)repositories[typeof(RhythmChartRepository)]).GetChart(id, chartType, difficulty);

		public static MemberData GetMemberData(Member member) => ((MemberDataRepository)repositories[typeof(MemberDataRepository)]).GetMemberData(member);

		public static LocalizationData GetLocalization(string key)
		{
			return GetData<LocalizationDataRepository>().Get(key);
		}

		public static string ToLocalizedText(this string key) => GetText(key);

		public static bool TryGetLocalization(string key, out LocalizationData data)
		{
			return GetData<LocalizationDataRepository>().TryGet(key, out data);
		}

		public static string GetLocalizedText(string key)
		{
			return GetLocalization(key).Text;
		}

		public static string GetMemberNameKey(this Member member) => GetLocalizedText(GetMemberData(member).NameKey);
	}
}
