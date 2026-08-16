using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MilliRhythm.Util
{
	public class TimeScaleScope : IDisposable
	{
		private CancellationTokenSource cts = new();

		public TimeScaleScope()
		{
			ChangeTimeScale().Forget();
		}

		public TimeScaleScope(float timeScale)
		{
			ChangeTimeScale(timeScale).Forget();
		}

		private async UniTaskVoid ChangeTimeScale(float timeScale = 0)
		{
			Time.timeScale = timeScale;
			await UniTask.WaitUntilCanceled(cts.Token);
			Time.timeScale = 1;
		}

		public void Dispose()
		{
			cts.Cancel();
			cts.Dispose();
			cts = null;
		}
	}
}
