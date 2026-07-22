using Cysharp.Threading.Tasks;

namespace MilliRhythm.Data.GameDataService
{
	public abstract class ScriptableObjectRepository : IGameDataRepository
	{
		public abstract UniTask LoadAsync();
	}
}
