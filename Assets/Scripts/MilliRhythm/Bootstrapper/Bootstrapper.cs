using Cysharp.Threading.Tasks;
using MilliRhythm.Data.GameDataService;
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
		}
	}
}
