using Cysharp.Threading.Tasks;
using MilliRhythm.Data.GameDataService;
using MilliRhythm.Rhythm;
using UnityEngine;

namespace MilliRhythm.Bootstrapper
{
	public class Bootstrapper : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OperateGame()
		{
			OperateGameAsync().Forget();
		}

		private static async UniTask OperateGameAsync()
		{
			await GameDataLoader.LoadAsync();
			Debug.Log(GameDataService.GetMusicData(1).Name);
			Debug.Log(GameDataService.GetChartData(1, ChartType.Beat, Difficulty.Easy).AudioClip.length);
		}
	}
}
