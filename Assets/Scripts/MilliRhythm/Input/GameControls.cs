using MilliRhythm.Input.Sources;
using R3;
using UnityEngine.InputSystem;

namespace MilliRhythm.Input
{
	public partial class GameControls : ControlBase
	{
		public ReadOnlyReactiveProperty<bool> Up => up;
		public ReadOnlyReactiveProperty<bool> Down => down;
		public ReadOnlyReactiveProperty<bool> Left => left;
		public ReadOnlyReactiveProperty<bool> Right => right;
		public ReadOnlyReactiveProperty<bool> Menu => menu;
		private readonly ReactiveProperty<bool> up = new();
		private readonly ReactiveProperty<bool> down = new();
		private readonly ReactiveProperty<bool> left = new();
		private readonly ReactiveProperty<bool> right = new();
		private readonly ReactiveProperty<bool> menu = new();

		public override void AddInputSource(IInputSource inputSource)
		{
			switch (inputSource)
			{
				case InputSystemInputSource inputSourceInputSystem:
					AddInputAdapter(inputSourceInputSystem, new InputSystemInputAdapter(inputSourceInputSystem.MilliRhythmInputs.Game, this));
					break;
				case GameTouchUIInputSource gameTouchUIInputSource:
					AddInputAdapter(gameTouchUIInputSource, new TouchUIInputAdapter(gameTouchUIInputSource, this));
					break;
			}
		}

		protected override void ResetAllInputs()
		{
			up.Value = false;
			down.Value = false;
			left.Value = false;
			right.Value = false;
			menu.Value = false;
		}

		public void Dispose()
		{
			up?.Dispose();
			down?.Dispose();
			left?.Dispose();
			right?.Dispose();
			menu?.Dispose();
		}
	}

	public partial class GameControls
	{
		private abstract class GameInputAdapter : IInputAdapter
		{
			private readonly GameControls controls;

			protected GameInputAdapter(GameControls controls)
			{
				this.controls = controls;
			}

			public abstract void Register();
			public abstract void Unregister();
			protected void OnUp(bool value) => controls.up.Value = value;
			protected void OnDown(bool value) => controls.down.Value = value;
			protected void OnLeft(bool value) => controls.left.Value = value;
			protected void OnRight(bool value) => controls.right.Value = value;
			protected void OnMenu(bool value) => controls.menu.Value = value;
		}

		private class InputSystemInputAdapter : GameInputAdapter, MilliRhythmInputs.IGameActions
		{
			private MilliRhythmInputs.GameActions actions;

			public InputSystemInputAdapter(MilliRhythmInputs.GameActions actions, GameControls controls) : base(controls)
			{
				this.actions = actions;
			}

			public override void Register()
			{
				actions.AddCallbacks(this);
			}

			public override void Unregister()
			{
				actions.RemoveCallbacks(this);
			}

			public void OnUp(InputAction.CallbackContext context) => OnUp(context.ReadValueAsButton());
			public void OnDown(InputAction.CallbackContext context) => OnDown(context.ReadValueAsButton());
			public void OnLeft(InputAction.CallbackContext context) => OnLeft(context.ReadValueAsButton());
			public void OnRight(InputAction.CallbackContext context) => OnRight(context.ReadValueAsButton());
			public void OnMenu(InputAction.CallbackContext context) => OnMenu(context.ReadValueAsButton());
		}

		private class TouchUIInputAdapter : GameInputAdapter
		{
			private readonly GameTouchUIInputSource inputSource;

			public TouchUIInputAdapter(GameTouchUIInputSource inputSource, GameControls controls) : base(controls)
			{
				this.inputSource = inputSource;
			}

			public override void Register()
			{
				inputSource.ButtonLeft.OnInput += OnLeft;
				inputSource.ButtonUp.OnInput += OnUp;
				inputSource.ButtonDown.OnInput += OnDown;
				inputSource.ButtonRight.OnInput += OnRight;
			}

			public override void Unregister()
			{
				inputSource.ButtonLeft.OnInput -= OnLeft;
				inputSource.ButtonUp.OnInput -= OnUp;
				inputSource.ButtonDown.OnInput -= OnDown;
				inputSource.ButtonRight.OnInput -= OnRight;
			}
		}
	}
}
