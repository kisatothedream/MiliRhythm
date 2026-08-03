using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MilliRhythm.User.Score;

namespace MilliRhythm.User
{
	public class UserProcessor
	{
		private readonly List<ISubProcessor> subProcessors = new List<ISubProcessor>();

		public UserProcessor()
		{
			subProcessors.Add(new ScoreProcessor());
		}

		internal Response ReceiveRequest(Request request)
		{
			var eventQueue = new Queue<IUserEvent>();
			var result = true;

			eventQueue.Enqueue(request.evt);

			while (eventQueue.Count > 0)
			{
				var evt = eventQueue.Dequeue();
				result &= Process(evt);
			}

			return new Response() { Result = result ? Result.Success : Result.Failure };
		}

		internal async UniTask Init()
		{
			foreach (var subProcessor in subProcessors)
			{
				await subProcessor.Init();
			}
		}

		//게임을 완전히 최초 구동했을 때, 데이터 세팅하는 함수
		internal async UniTask SetGameInitialState()
		{
			var tasks = new List<UniTask>();
			foreach (var subProcessor in subProcessors)
			{
				tasks.Add(subProcessor.SetGameInitialData());
			}

			await UniTask.WhenAll(tasks);
		}

		private bool Process(IUserEvent evt)
		{
			var result = true;
			foreach (var subProcessor in subProcessors)
			{
				result &= subProcessor.Process(evt);
			}

			return result;
		}
	}
}
