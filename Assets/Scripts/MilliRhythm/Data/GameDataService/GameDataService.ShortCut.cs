using MilliRhythm.Data.Repository;

namespace MilliRhythm.Data.GameDataService
{
	public static partial class GameDataService
	{
		public static ItemData GetItem(int itemId) => ((ItemDataRepository)repositories[typeof(ItemDataRepository)]).Get(itemId);
		public static string GetText(string key) => ((LocalizationDataRepository)repositories[typeof(LocalizationDataRepository)]).GetText(key);
		public static MusicData GetMusicData(int id) => ((MusicDataRepository)repositories[typeof(MusicDataRepository)]).GetMusicDataById(id);

		public static LocalizationData GetLocalization(string key)
		{
			return GetData<LocalizationDataRepository>().Get(key);
		}

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
