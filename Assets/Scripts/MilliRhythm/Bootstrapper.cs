using System;
using Cysharp.Threading.Tasks;
using MilliRhythm.Config;
using MilliRhythm.Data.GameDataService;
using MilliRhythm.Input;
using MilliRhythm.Rhythm;
using MilliRhythm.Scene;
using MilliRhythm.Scene.Contracts;
using MilliRhythm.TrackSelector;
using MilliRhythm.UI.Util;
using MilliRhythm.User;
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
			InputManager.Instance.Load();
			ConfigManager.Instance.Load();
			await GameDataLoader.LoadAsync();
			await UserManager.Init();
			SceneController.Instance.Initialize(CreateScene);
			SceneController.Instance.RequestChangeScene(new MusicSelectorSceneParameter());

			await UniTask.WaitUntil(() => FadeTransition.Instance != null && FadeTransition.Instance.Initialized);
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
