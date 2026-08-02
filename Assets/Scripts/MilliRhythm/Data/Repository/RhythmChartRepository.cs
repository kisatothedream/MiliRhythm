using System.Linq;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.GameDataService;
using MilliRhythm.Rhythm;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MilliRhythm.Data.Repository
{
	public class RhythmChartRepository : IGameDataRepository
	{
		private AsyncOperationHandle<RhythmChartDataCollection> handle;
		private RhythmChartDataCollection rhythmChartDataCollection;
		private const string FileKey = "ChartDataRepository";

		public async UniTask LoadAsync()
		{
			handle = Addressables.LoadAssetAsync<RhythmChartDataCollection>(FileKey);
			rhythmChartDataCollection = await handle.Task;
		}

		public void Release()
		{
			if (handle.IsValid())
				Addressables.Release(handle);
		}

		public RhythmChart GetChart(int id, ChartType chartType, Difficulty difficulty) =>
			rhythmChartDataCollection.ChartDataList.First(item => item.MusicId == id && item.ChartType == chartType && item.Difficulty == difficulty);
	}
}
