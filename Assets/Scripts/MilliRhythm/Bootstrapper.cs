using System;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.GameDataService;
using MilliRhythm.Rhythm;
using MilliRhythm.Scene;
using MilliRhythm.Scene.Contracts;
using MilliRhythm.TrackSelector;
using UnityEngine;

namespace MilliRhythm
{
	public static class Bootstrapper
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void BeforeSceneLoad()
		{
			InitializeGameAsync().Forget();
		}

		private static async UniTask InitializeGameAsync()
		{
			await GameDataLoader.LoadAsync();
			SceneController.Instance.Initialize(CreateScene);
			SceneController.Instance.RequestChangeScene(new RhythmGameSceneParameter(1, ChartType.Melody, Difficulty.Easy));
		}

		private static GameSceneBase CreateScene(IGameSceneParameter sceneParameter)
		{
			return sceneParameter switch
			{
				RhythmGameSceneParameter => new RhythmGameScene(),
				MusicSelectorSceneParameter => new MusicSelectorScene(),
				_ => throw new ArgumentOutOfRangeException(nameof(sceneParameter), sceneParameter, null)
			};
		}
	}
}
