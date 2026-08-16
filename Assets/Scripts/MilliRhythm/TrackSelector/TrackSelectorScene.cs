using System;
using Cysharp.Threading.Tasks;
using MilliRhythm.CustomException;
using MilliRhythm.Scene.Contracts;
using MilliRhythm.UI.TrackSelectorUI;
using MilliRhythm.UI.Util;
using Object = UnityEngine.Object;

namespace MilliRhythm.TrackSelector
{
	public class MusicSelectorScene : GameSceneBase
	{
		public override int SceneIndex => 1;
		private TrackSelectorUIController trackSelectorUIController;
		private MusicSelectorSceneParameter param;

		public override async UniTask Load()
		{
			trackSelectorUIController = Object.FindFirstObjectByType<TrackSelectorUIController>();
			if (trackSelectorUIController == null)
			{
				throw new MissingSceneComponentException(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, typeof(TrackSelectorUIController));
			}
		}

		public override async UniTask Init(IGameSceneParameter parameter)
		{
			trackSelectorUIController.Set();
			if (parameter is not MusicSelectorSceneParameter sceneParameter) throw new ArgumentException($"Parameter must be of type {typeof(MusicSelectorSceneParameter)}", nameof(parameter));
			if (sceneParameter.LastMusicId == -1) return;
			param = sceneParameter;
			trackSelectorUIController.SelectLastTrack(param.LastMusicId);
		}

		public override void Start()
		{
		}

		public override void Finish()
		{
			trackSelectorUIController.Finish();
		}

		public override async UniTask PlayEnterTransition()
		{
			await FadeTransition.Instance.FadeInAsync();
		}

		public override async UniTask PlayExitTransition()
		{
			await FadeTransition.Instance.FadeOutAsync();
		}
	}
}
