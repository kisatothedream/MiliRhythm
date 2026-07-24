using System;
using Cysharp.Threading.Tasks;
using MilliRhythm.Scene.Contracts;
using UnityEngine.SceneManagement;

namespace MilliRhythm.Scene
{
	public class SceneController
	{
		public static SceneController Instance => instance ??= new SceneController();
		private static SceneController instance;
		private Func<IGameSceneParameter, GameSceneBase> sceneFactory;

		private bool isLoading;
		private GameSceneBase currentScene;


		public void Initialize(Func<IGameSceneParameter, GameSceneBase> factory)
		{
			sceneFactory = factory;
		}

		public void RequestChangeScene(IGameSceneParameter sceneParameter, IGameSceneResult sceneResult = null)
		{
			LoadSceneAsync(sceneParameter).Forget();
		}

		private async UniTask LoadSceneAsync(IGameSceneParameter sceneParameter)
		{
			isLoading = true;
			currentScene?.Finish();

			currentScene = sceneFactory.Invoke(sceneParameter);
			await SceneManager.LoadSceneAsync(currentScene.SceneName, LoadSceneMode.Single).ToUniTask();

			currentScene.Load();
			currentScene.Init();
			isLoading = false;

			currentScene.Start();
		}
	}
}
