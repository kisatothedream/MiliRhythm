using MilliRhythm.Input.Sources;
using UnityEngine;

namespace MilliRhythm.Input
{
	public class InputManager
	{
		public static InputManager Instance { get; private set; }
		private readonly MilliRhythmInputs milliRhythmInputs;
		private MilliRhythmInputs.GameActions characterActions;

		private InputSystemInputSource inputSystemInputSource;
		private GameControls GameControls { get; set; }
		private UIControls UIControls { get; set; }

		private InputManager()
		{
			milliRhythmInputs = new MilliRhythmInputs();
			milliRhythmInputs.Enable();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatic()
		{
			Instance.Dispose();
			Instance = null;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void BeforeSceneLoaded()
		{
			Instance = new InputManager();
			Instance.Init();
		}

		private void Init()
		{
			inputSystemInputSource = new InputSystemInputSource(milliRhythmInputs);

			//Initialize Feature Controls
			GameControls = new GameControls();
			UIControls = new UIControls();
			UIInputManager.Instance.Init(UIControls);

			AddInputSource(inputSystemInputSource);
		}

		private void Dispose()
		{
			GameControls?.Dispose();
			UIControls?.Dispose();
		}

		public void AddInputSource(IInputSource inputSource)
		{
			GameControls.AddInputSource(inputSource);
			UIControls.AddInputSource(inputSource);
		}

		public void RemoveInputSource(IInputSource inputSource)
		{
			GameControls.RemoveInputSource(inputSource);
			UIControls.RemoveInputSource(inputSource);
		}
	}
}
