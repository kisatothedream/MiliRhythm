using R3;

namespace MilliRhythm.Rhythm
{
	public partial class RhythmGamePlayer
	{
		private void RegisterInputs()
		{
			disposable = new CompositeDisposable();
			controls.Left.Subscribe(OnLeft).AddTo(disposable);
			controls.Up.Subscribe(OnUp).AddTo(disposable);
			controls.Down.Subscribe(OnDown).AddTo(disposable);
			controls.Right.Subscribe(OnRight).AddTo(disposable);
		}

		private void Unregister()
		{
			disposable.Dispose();
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
