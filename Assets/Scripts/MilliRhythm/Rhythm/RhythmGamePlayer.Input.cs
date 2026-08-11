using R3;

namespace MilliRhythm.Rhythm
{
	public partial class RhythmGamePlayer
	{
		private void RegisterInputs()
		{
			inputDisposable = new CompositeDisposable();
			controls.Left.Subscribe(OnLeft).AddTo(inputDisposable);
			controls.Up.Subscribe(OnUp).AddTo(inputDisposable);
			controls.Down.Subscribe(OnDown).AddTo(inputDisposable);
			controls.Right.Subscribe(OnRight).AddTo(inputDisposable);
			controls.Menu.Subscribe(OnMenu).AddTo(inputDisposable);
		}

		private void UnregisterInputs()
		{
			inputDisposable?.Dispose();
		}

		private void OnLeft(bool pressed)
		{
			if (pressed) OnPressKey(NoteType.Left.ToInt());
			isLaneHeld[0] = pressed;
		}

		private void OnUp(bool pressed)
		{
			if (pressed) OnPressKey(NoteType.Up.ToInt());
			isLaneHeld[1] = pressed;
		}

		private void OnDown(bool pressed)
		{
			if (pressed) OnPressKey(NoteType.Down.ToInt());
			isLaneHeld[2] = pressed;
		}

		private void OnRight(bool pressed)
		{
			if (pressed) OnPressKey(NoteType.Right.ToInt());
			isLaneHeld[3] = pressed;
		}

		private void OnMenu(bool pressed)
		{
			if (pressed) uiController.DisplayPauseUI();
		}
	}
}
