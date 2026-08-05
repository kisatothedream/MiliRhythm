using System;
using System.Collections.Generic;
using MilliRhythm.Input.Sources;

namespace MilliRhythm.Input
{
	public abstract class ControlBase : IControls
	{
		private readonly Dictionary<IInputSource, IInputAdapter> inputAdapters = new();

		private int blockCounter;
		private bool isEnabled => blockCounter == 0;

		public abstract void AddInputSource(IInputSource inputSource);
		public void RemoveInputSource(IInputSource inputSource) => RemoveInputAdapter(inputSource);

		protected void AddInputAdapter(IInputSource inputSource, IInputAdapter inputAdapter)
		{
			inputAdapters.Add(inputSource, inputAdapter);
			if (isEnabled)
				inputAdapter.Register();
		}

		private void RemoveInputAdapter(IInputSource inputSource)
		{
			if (!inputAdapters.TryGetValue(inputSource, out IInputAdapter adapter)) return;
			if (isEnabled) adapter.Unregister();
			inputAdapters.Remove(inputSource);
		}

		private void SetEnable(bool enable)
		{
			if (enable) OnEnable();
			else OnDisable();
		}

		private void OnEnable()
		{
			foreach (var inputAdapter in inputAdapters.Values)
			{
				inputAdapter.Register();
			}
		}

		private void OnDisable()
		{
			foreach (var inputAdapter in inputAdapters.Values)
			{
				inputAdapter.Unregister();
			}

			ResetAllInputs();
		}

		public void Block()
		{
			blockCounter--;

			if (blockCounter == 1)
			{
				SetEnable(true);
			}
		}

		public void Unblock()
		{
			if (blockCounter <= 0)
			{
				throw new InvalidOperationException("Input is not blocked.");
			}

			blockCounter--;

			if (blockCounter == 0)
			{
				SetEnable(true);
			}
		}

		protected abstract void ResetAllInputs();
	}

	public interface IControls
	{
	}
}
