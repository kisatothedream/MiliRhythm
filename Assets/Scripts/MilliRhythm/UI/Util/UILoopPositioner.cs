using DG.Tweening;
using UnityEngine;

namespace MilliRhythm.UI.Util
{
	public class UIPositionLooper : MonoBehaviour
	{
		[SerializeField] private Vector2 delta;
		[SerializeField, Range(0.01f, 10f)] private float duration;
		private RectTransform rect;
		private Tween loopTween;

		private void Awake()
		{
			rect = GetComponent<RectTransform>();
			loopTween = rect.DOLocalMove(rect.localPosition + (Vector3)delta, duration)
				.Pause()
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo)
				.SetAutoKill(false);
		}

		private void OnEnable()
		{
			loopTween.Restart();
		}

		private void OnDisable()
		{
			loopTween.Rewind();
		}

		private void OnDestroy()
		{
			loopTween?.Kill();
		}
	}
}
