using Cysharp.Threading.Tasks;

namespace MilliRhythm.Data.GameDataService
{
	public interface IGameDataRepository
	{
		public UniTask LoadAsync();
		public void Release();
	}
}
