using Cysharp.Threading.Tasks;
using MilliRhythm.CustomException;
using MilliRhythm.Scene.Contracts;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	public sealed class RhythmGameScene : GameSceneBase
	{
		public override int SceneIndex => 1;

		private RhythmGamePlayer rhythmGamePlayer;

		public override async UniTask Load()
		{
			rhythmGamePlayer = Object.FindFirstObjectByType<RhythmGamePlayer>();
			if (rhythmGamePlayer == null)
			{
				throw new MissingSceneComponentException(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, typeof(RhythmGamePlayer));
			}
		}

		public override void Init(IGameSceneParameter parameter)
		{
		}

		public override void Start()
		{
		}

		public override void Finish()
		{
		}

		public override async UniTask PlayEnterTransition()
		{
		}

		public override async UniTask PlayExitTransition()
		{
		}
	}
}
