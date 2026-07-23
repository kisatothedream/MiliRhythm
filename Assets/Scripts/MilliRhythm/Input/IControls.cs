using System.Collections.Generic;
using MilliRhythm.Input.Sources;

namespace MilliRhythm.Input
{
	public abstract class ControlBase : IControls
	{
		private readonly Dictionary<IInputSource, IInputAdapter> inputAdapters = new();

		public abstract void AddInputSource(IInputSource inputSource);
		public void RemoveInputSource(IInputSource inputSource) => RemoveInputAdapter(inputSource);

		protected void AddInputAdapter(IInputSource inputSource, IInputAdapter inputAdapter)
		{
			inputAdapters.Add(inputSource, inputAdapter);
			inputAdapter.Register();
		}

		private void RemoveInputAdapter(IInputSource inputSource)
		{
			if (!inputAdapters.TryGetValue(inputSource, out IInputAdapter adapter)) return;
			adapter.Unregister();
			inputAdapters.Remove(inputSource);
		}

		public void SetEnable(bool enable)
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
				inputAdapter.Register();
			}
			ResetAllInputs();
		}

		protected abstract void ResetAllInputs();
	}

	public interface IControls
	{
	}
}
