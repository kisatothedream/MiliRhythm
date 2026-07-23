using MilliRhythm.Input.TouchUI;
using UnityEngine;

namespace MilliRhythm.Input.Sources
{
	public class GameTouchUIInputSource : MonoBehaviour, IInputSource
	{
		[field: SerializeField] public TouchNoteButton ButtonUp { get; private set; }
		[field: SerializeField] public TouchNoteButton ButtonDown { get; private set; }
		[field: SerializeField] public TouchNoteButton ButtonLeft { get; private set; }
		[field: SerializeField] public TouchNoteButton ButtonRight { get; private set; }

		private void OnEnable()
		{
			InputManager.Instance.AddInputSource(this);
		}

		private void OnDisable()
		{
			InputManager.Instance.RemoveInputSource(this);
		}
	}
}
