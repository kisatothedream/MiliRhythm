using R3;

namespace MilliRhythm.Rhythm
{
	public partial class RhythmGamePlayer 
	{ 
		private void RegisterCharacter()
		{
			characterDisposable = new CompositeDisposable();
			controls.Left.Subscribe(character.OnLeft).AddTo(inputDisposable);
			controls.Up.Subscribe(character.OnUp).AddTo(inputDisposable);
			controls.Down.Subscribe(character.OnDown).AddTo(inputDisposable);
			controls.Right.Subscribe(character.OnRight).AddTo(inputDisposable);
		}

		private void UnregisterCharacter()
		{
			inputDisposable.Dispose();
		}
	}
}
