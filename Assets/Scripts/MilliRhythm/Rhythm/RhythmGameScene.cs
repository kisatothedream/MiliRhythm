using System;
using Cysharp.Threading.Tasks;
using MilliRhythm.CustomException;
using MilliRhythm.Scene.Contracts;
using Object = UnityEngine.Object;

namespace MilliRhythm.Rhythm
{
	public sealed class RhythmGameScene : GameSceneBase
	{
		public override int SceneIndex => 2;

		private RhythmGamePlayer rhythmGamePlayer;

		public override async UniTask Load()
		{
			rhythmGamePlayer = Object.FindFirstObjectByType<RhythmGamePlayer>();
			if (rhythmGamePlayer == null)
			{
				throw new MissingSceneComponentException(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, typeof(RhythmGamePlayer));
			}
		}

		public override async UniTask Init(IGameSceneParameter parameter)
		{
			if (parameter is not RhythmGameSceneParameter sceneParameter)
			{
				throw new ArgumentException($"Scene parameter is not {typeof(RhythmGameSceneParameter)}");
			}

			await rhythmGamePlayer.Init();
		}

		public override void Start()
		{
			rhythmGamePlayer.OnStart();
		}

		public override void Finish()
		{
			rhythmGamePlayer.Finish();
		}

		public override async UniTask PlayEnterTransition()
		{
		}

		public override async UniTask PlayExitTransition()
		{
		}
	}
}
