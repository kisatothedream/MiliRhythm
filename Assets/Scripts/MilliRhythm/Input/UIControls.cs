using MilliRhythm.Input.Sources;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MilliRhythm.Input
{
	public partial class UIControls : ControlBase
	{
		public ReadOnlyReactiveProperty<Vector2> Navigate => navigate;
		public ReadOnlyReactiveProperty<bool> Submit => submit;
		public ReadOnlyReactiveProperty<bool> Cancel => cancel;
		public ReadOnlyReactiveProperty<bool> Config => config;
		public ReadOnlyReactiveProperty<bool> Filter => filter;
		private readonly ReactiveProperty<Vector2> navigate = new();
		private readonly ReactiveProperty<bool> submit = new();
		private readonly ReactiveProperty<bool> cancel = new();
		private readonly ReactiveProperty<bool> config = new();
		private readonly ReactiveProperty<bool> filter = new();

		public override void AddInputSource(IInputSource inputSource)
		{
			switch (inputSource)
			{
				case InputSystemInputSource inputSourceInputSystem:
					AddInputAdapter(inputSourceInputSystem, new InputSystemInputAdapter(inputSourceInputSystem.MilliRhythmInputs.UI, this));
					break;
			}
		}

		protected override void ResetAllInputs()
		{
			navigate.Value = default;
			submit.Value = false;
			cancel.Value = false;
			config.Value = false;
			filter.Value = false;
		}

		public void Dispose()
		{
			navigate?.Dispose();
			submit?.Dispose();
			cancel?.Dispose();
			config?.Dispose();
			filter?.Dispose();
		}
	}

	public partial class UIControls
	{
		private abstract class UIInputAdapter : IInputAdapter
		{
			private readonly UIControls controls;

			protected UIInputAdapter(UIControls controls)
			{
				this.controls = controls;
			}

			public abstract void Register();
			public abstract void Unregister();

			protected void OnNavigate(Vector2 value) => controls.navigate.Value = value;
			protected void OnSubmit(bool value) => controls.submit.Value = value;
			protected void OnCancel(bool value) => controls.cancel.Value = value;
			protected void OnConfig(bool value) => controls.config.Value = value;
			protected void OnFilter(bool value) => controls.filter.Value = value;
		}

		private class InputSystemInputAdapter : UIInputAdapter, MilliRhythmInputs.IUIActions
		{
			private MilliRhythmInputs.UIActions actions;

			public InputSystemInputAdapter(MilliRhythmInputs.UIActions actions, UIControls controls) : base(controls)
			{
				this.actions = actions;
			}

			public void OnNavigate(InputAction.CallbackContext context) => OnNavigate(context.ReadValue<Vector2>());
			public void OnSubmit(InputAction.CallbackContext context) => OnSubmit(context.ReadValueAsButton());
			public void OnCancel(InputAction.CallbackContext context) => OnCancel(context.ReadValueAsButton());
			public void OnConfig(InputAction.CallbackContext context) => OnConfig(context.ReadValueAsButton());
			public void OnFilter(InputAction.CallbackContext context) => OnFilter(context.ReadValueAsButton());

			public override void Register()
			{
				actions.AddCallbacks(this);
			}

			public override void Unregister()
			{
				actions.RemoveCallbacks(this);
			}
		}
	}
}
