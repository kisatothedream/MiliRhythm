using System;
using MilliRhythm.Input.Sources;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MilliRhythm.Input
{
	public partial class InputManager
	{
		public static InputManager Instance { get; private set; }
		private readonly MilliRhythmInputs milliRhythmInputs;
		private MilliRhythmInputs.GameActions characterActions;

		private InputSystemInputSource inputSystemInputSource;
		public GameControls GameControls { get; private set; }
		private UIControls UIControls { get; set; }

		private InputManager()
		{
			milliRhythmInputs = new MilliRhythmInputs();
			milliRhythmInputs.Enable();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatic()
		{
			Instance?.Dispose();
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

			inputAsset = milliRhythmInputs.asset;
			LoadInputRebinding();

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

			CancelRebind();
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

	public partial class InputManager
	{
		private const string InputBindingSaveKey = "InputBindingOverrides";

		private InputActionAsset inputAsset;
		private InputActionRebindingExtensions.RebindingOperation rebindOperation;

		private void LoadInputRebinding()
		{
			if (!PlayerPrefs.HasKey(InputBindingSaveKey))
			{
				return;
			}

			var json = PlayerPrefs.GetString(InputBindingSaveKey);
			inputAsset.LoadBindingOverridesFromJson(json);
		}

		public void Rebind(InputAction action, int bindingIndex, Action<string> onComplete = null, Action onCancel = null)
		{
			CancelRebind();

			var actionMap = action.actionMap;
			var wasEnabled = actionMap?.enabled ?? action.enabled;

			if (actionMap != null)
			{
				actionMap.Disable();
			}
			else
			{
				action.Disable();
			}

			rebindOperation = action
				.PerformInteractiveRebinding(bindingIndex)
				.WithControlsHavingToMatchPath("<Keyboard>")
				.WithCancelingThrough("<Keyboard>/escape")
				.OnComplete(_ =>
				{
					Save();
					var displayName = action.GetBindingDisplayString(bindingIndex);
					FinishRebind(action, actionMap, wasEnabled);
					onComplete?.Invoke(displayName);
				})
				.OnCancel(_ =>
				{
					FinishRebind(action, actionMap, wasEnabled);
					onCancel?.Invoke();
				});

			rebindOperation.Start();
		}

		/*
		public void RebindLeft()
		{
			var action = controls.Gameplay.Left;
			var bindingIndex = GetKeyboardBindingIndex(action);

			rebindService.StartRebind(
				action,
				bindingIndex,
				keyName => leftKeyText.text = keyName);
		}

		private int GetKeyboardBindingIndex(InputAction action)
		{
			var bindingIndex = action.GetBindingIndex(InputBinding.MaskByGroup("Keyboard"));

			if (bindingIndex < 0)
			{
				throw new InvalidOperationException($"{action.name}에 Keyboard 바인딩이 없습니다.");
			}

			return bindingIndex;
		}
		keyText.text = action.GetBindingDisplayString(bindingIndex);
		 */

		public void ResetAll()
		{
			CancelRebind();
			inputAsset.RemoveAllBindingOverrides();
			PlayerPrefs.DeleteKey(InputBindingSaveKey);
			PlayerPrefs.Save();
		}

		public string GetBindingDisplayName(InputAction action, int bindingIndex)
		{
			return action.GetBindingDisplayString(bindingIndex);
		}

		private void Save()
		{
			var json = inputAsset.SaveBindingOverridesAsJson();
			PlayerPrefs.SetString(InputBindingSaveKey, json);
			PlayerPrefs.Save();
		}

		private void FinishRebind(InputAction action, InputActionMap actionMap, bool wasEnabled)
		{
			rebindOperation?.Dispose();
			rebindOperation = null;

			if (!wasEnabled)
			{
				return;
			}

			if (actionMap != null)
			{
				actionMap.Enable();
			}
			else
			{
				action.Enable();
			}
		}

		private void CancelRebind()
		{
			if (rebindOperation == null)
			{
				return;
			}

			rebindOperation.Cancel();
			rebindOperation.Dispose();
			rebindOperation = null;
		}
	}
}
