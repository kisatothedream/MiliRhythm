using System;
using MilliRhythm.Rhythm;
using MilliRhythm.Scene;
using MilliRhythm.Scene.Contracts;
using UnityEngine;

namespace MilliRhythm
{
	public static class Bootstrapper
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void BeforeSceneLoad()
		{
		}

		private static void InitializeGame()
		{
			SceneController.Instance.Initialize(CreateScene);
		}

		private static GameSceneBase CreateScene(IGameSceneParameter sceneParameter)
		{
			return sceneParameter switch
			{
				RhythmGameSceneParameter => new RhythmGameScene(),
				_ => throw new ArgumentOutOfRangeException(nameof(sceneParameter), sceneParameter, null)
			};
		}
	}
}
