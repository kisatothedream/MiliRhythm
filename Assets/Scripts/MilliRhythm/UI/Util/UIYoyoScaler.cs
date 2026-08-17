using DG.Tweening;
using UnityEngine;

namespace MilliRhythm.UI.Util
{
	public class UIYoyoScaler : MonoBehaviour
	{
		[SerializeField, Range(0.01f, 10f)] private float targetScale;
		[SerializeField, Range(0.01f, 10f)] private float duration;
		private RectTransform rect;
		private Tween yoyoTween;

		private void Awake()
		{
			rect = GetComponent<RectTransform>();
			yoyoTween = rect.DOScale(targetScale, duration)
				.Pause()
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo)
				.SetUpdate(true)
				.SetAutoKill(false);
		}

		private void OnEnable()
		{
			yoyoTween.Restart();
		}

		private void OnDisable()
		{
			yoyoTween.Rewind();
		}

		private void OnDestroy()
		{
			yoyoTween?.Kill();
		}
	}
}
