using Cysharp.Threading.Tasks;
using MilliRhythm.CustomException;
using MilliRhythm.Scene.Contracts;
using MilliRhythm.UI.TrackSelectorUI;
using UnityEngine;

namespace MilliRhythm.TrackSelector
{
	public class MusicSelectorScene : GameSceneBase
	{
		public override int SceneIndex => 1;
		private TrackSelectorUIController trackSelectorUIController;

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
		}

		public override async UniTask PlayExitTransition()
		{
		}
	}
}
