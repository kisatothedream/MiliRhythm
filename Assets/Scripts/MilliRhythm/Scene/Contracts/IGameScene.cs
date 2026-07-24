namespace MilliRhythm.Scene.Contracts
{
	public abstract class GameSceneBase : IGameScene
	{
		public abstract string SceneName { get; }
		public abstract void Load();
		public abstract void Init();
		public abstract void Start();
		public abstract void Finish();
		public abstract void PlayEnterTransition();
		public abstract void PlayExitTransition();
	}

	public interface IGameScene
	{
		//씬의 기본 동작, 리소스 로드
		public void Load();

		//이번 씬 세션에서 사용할 요소 로드
		public void Init();
		public void Start();
		public void Finish();

		public void PlayEnterTransition();
		public void PlayExitTransition();
	}
}
