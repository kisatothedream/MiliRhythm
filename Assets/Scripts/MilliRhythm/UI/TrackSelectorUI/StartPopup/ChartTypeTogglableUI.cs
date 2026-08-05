using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.TrackSelectorUI.StartPopup
{
	public class ChartTypeTogglableUI : MonoBehaviour
	{
		[SerializeField] private Image frame;
		private CanvasGroup canvasGroup;
		[SerializeField] private Color originalColor;
		[SerializeField] private Color fadedColor;

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		public void SetSelected(bool value)
		{
			frame.color = value ? originalColor : fadedColor;
			canvasGroup.alpha = value ? 1f : 0.5f;
		}
	}
}
