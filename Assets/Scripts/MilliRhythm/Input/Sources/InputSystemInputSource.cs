namespace MilliRhythm.Input.Sources
{
	public class InputSystemInputSource : IInputSource
	{
		public readonly MilliRhythmInputs MilliRhythmInputs;
		public InputSystemInputSource(MilliRhythmInputs techLabsInputs) => MilliRhythmInputs = techLabsInputs;
	}
}
