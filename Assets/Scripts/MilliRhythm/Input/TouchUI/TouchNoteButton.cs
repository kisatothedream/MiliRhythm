using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MilliRhythm.Input.TouchUI
{
	public class TouchNoteButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] private RectTransform knob;

		public event Action<bool> OnInput;

		public void OnPointerDown(PointerEventData eventData)
		{
			OnInput?.Invoke(true);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			OnInput?.Invoke(false);
		}
	}
}
