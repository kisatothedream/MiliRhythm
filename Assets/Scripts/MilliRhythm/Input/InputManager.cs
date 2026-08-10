using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MilliRhythm.Config;
using MilliRhythm.Input.Sources;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace MilliRhythm.Input
{
	public partial class InputManager
	{
		public static InputManager Instance { get; private set; }
		private readonly MilliRhythmInputs milliRhythmInputs;
		private MilliRhythmInputs.GameActions gameActions;

		private InputSystemInputSource inputSystemInputSource;
		public GameControls GameControls { get; private set; }
		private UIControls UIControls { get; set; }
		public event Action<InputDeviceType> OnChangeDeviceEvent;

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

			gameActions = milliRhythmInputs.Game;
			AddInputSource(inputSystemInputSource);

			ConfigManager.Instance.OnKeyLayoutChangedAction += ChangeKeyLayout;
			InputSystem.onEvent += DetectInputDevice;
		}

		private void Dispose()
		{
			GameControls?.Dispose();
			UIControls?.Dispose();

			ConfigManager.Instance.OnKeyLayoutChangedAction -= ChangeKeyLayout;
			CancelRebind();
			InputSystem.onEvent -= DetectInputDevice;
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

		public async UniTask BlockInputAsync(CancellationToken token)
		{
			BlockControls();
			await UniTask.WaitUntilCanceled(token);
			UnblockControls();
		}

		private void BlockControls()
		{
			GameControls.Block();
			UIControls.Block();
		}

		private void UnblockControls()
		{
			GameControls.Unblock();
			UIControls.Unblock();
		}


		private void DetectInputDevice(InputEventPtr eventPtr, InputDevice device)
		{
			if (device != null && eventPtr.IsA<StateEvent>())
			{
				if (device is Keyboard)
				{
					OnChangeDeviceEvent?.Invoke(InputDeviceType.Keyboard);
				}
				else if (device is Gamepad)
				{
					OnChangeDeviceEvent?.Invoke(InputDeviceType.Gamepad);
				}
			}
		}
	}

	public class InputBlockScope : IDisposable
	{
		private CancellationTokenSource cts = new();

		public InputBlockScope() => InputManager.Instance.BlockInputAsync(cts.Token).Forget();

		public void Dispose()
		{
			cts.Cancel();
			cts.Dispose();
			cts = null;
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

		private void ChangeKeyLayout(KeyLayout keyLayout)
		{
			switch (keyLayout)
			{
				case KeyLayout.WASD:
					ApplyWasdLayout();
					break;
				case KeyLayout.SDKL:
					ApplySdklLayout();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(keyLayout), keyLayout, null);
			}
		}

		private void ApplySdklLayout()
		{
			Debug.Log(gameActions.Left);
			gameActions.Left.ApplyBindingOverride(0, "<Keyboard>/s");
			gameActions.Up.ApplyBindingOverride(0, "<Keyboard>/d");
			gameActions.Down.ApplyBindingOverride(0, "<Keyboard>/k");
			gameActions.Right.ApplyBindingOverride(0, "<Keyboard>/l");
		}

		private void ApplyWasdLayout()
		{
			gameActions.Left.RemoveBindingOverride(0);
			gameActions.Up.RemoveBindingOverride(0);
			gameActions.Down.RemoveBindingOverride(0);
			gameActions.Right.RemoveBindingOverride(0);
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
