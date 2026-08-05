using Cysharp.Threading.Tasks;

namespace MilliRhythm.User
{
	public interface ISubProcessor
	{
		public UniTask SetGameInitialData();
		public UniTask Init();
		public bool Process(IUserEvent evt);
	}
}
