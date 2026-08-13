using System.Collections.Generic;
using MilliRhythm.Data.Domain;
using MilliRhythm.Data.Repository;
using MilliRhythm.Rhythm;

namespace MilliRhythm.Data.GameDataService
{
	public static partial class GameDataService
	{
		public static string GetText(string key) => ((LocalizationDataRepository)repositories[typeof(LocalizationDataRepository)]).GetText(key);
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
	}
}
