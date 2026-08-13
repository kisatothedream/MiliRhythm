using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.Util
{
	public class UIColorFader : MonoBehaviour
	{
		private Image image;
		private Color originalColor;
		private Color targetColor;
		private Tween faderTween;


		private void Awake()
		{
			image = GetComponent<Image>();

			originalColor = image.color;
			targetColor = originalColor + new Color(0.15f, -0.15f, -0.15f, 0f);

			// faderTween = image
			// 	.DOColor(targetColor, 0.5f)
			// 	.SetEase(Ease.InOutSine)
			// 	.SetLoops(-1, LoopType.Yoyo)
			// 	.SetAutoKill(false)
			// 	.Pause();
		}

		public void OnEnable()
		{
			faderTween.Restart();
		}

		public void OnDisable()
		{
			faderTween.Rewind();
		}

		private void OnDestroy()
		{
			faderTween?.Kill();
		}
	}
}
