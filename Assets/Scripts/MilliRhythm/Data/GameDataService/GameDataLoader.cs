using Cysharp.Threading.Tasks;

namespace MilliRhythm.Data.GameDataService
{
	public static class GameDataLoader
	{
		public static async UniTask LoadAsync()
		{
			if (GameDataService.Initialized)
				return;

			await GameDataService.Initialize();

			GameDataService.Initialized = true;
		}
	}
}
