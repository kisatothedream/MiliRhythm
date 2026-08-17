using TMPro;
using UnityEngine;

namespace MilliRhythm.UI.Util
{
	public class FrameRateViewer : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI fpsText;

		private float deltaTime;

		private void Update()
		{
			deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

			var fps = 1f / deltaTime;
			fpsText.text = $"{fps:0} FPS";
		}
	}
}
