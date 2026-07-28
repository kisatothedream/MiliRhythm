using Cysharp.Threading.Tasks;

namespace MilliRhythm.Scene.Contracts
{
	public abstract class GameSceneBase : IGameScene
	{
		public abstract int SceneIndex { get; }
		public abstract UniTask Load();
		public abstract UniTask Init(IGameSceneParameter parameter);
		public abstract void Start();
		public abstract void Finish();
		public abstract UniTask PlayEnterTransition();
		public abstract UniTask PlayExitTransition();
	}

	public interface IGameScene
	{
		//씬의 기본 동작, 리소스 로드
		public UniTask Load();

		//이번 씬 세션에서 사용할 요소 로드
		public UniTask Init(IGameSceneParameter parameter);
		public void Start();
		public void Finish();

		public UniTask PlayEnterTransition();
		public UniTask PlayExitTransition();
	}
}
