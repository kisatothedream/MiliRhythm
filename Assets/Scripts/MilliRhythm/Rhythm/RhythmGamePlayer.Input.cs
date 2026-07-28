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
		}

		private void UnregisterInputs()
		{
			inputDisposable.Dispose();
		}

		private void OnLeft(bool pressed)
		{
			if (pressed) JudgeNotesDown(NoteType.Left.GetLane());
			else JudgeNotesUp(NoteType.Left.GetLane());
		}

		private void OnUp(bool pressed)
		{
			if (pressed) JudgeNotesDown(NoteType.Up.GetLane());
			else JudgeNotesUp(NoteType.Up.GetLane());
		}

		private void OnDown(bool pressed)
		{
			if (pressed) JudgeNotesDown(NoteType.Down.GetLane());
			else JudgeNotesUp(NoteType.Down.GetLane());
		}

		private void OnRight(bool pressed)
		{
			if (pressed) JudgeNotesDown(NoteType.Right.GetLane());
			else JudgeNotesUp(NoteType.Right.GetLane());
		}
	}
}
