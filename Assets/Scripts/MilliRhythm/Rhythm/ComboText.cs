using System;
using DG.Tweening;
using MilliRhythm.Data.Domain;
using TMPro;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	public class ComboText : MonoBehaviour
	{
		[SerializeField] private TextMeshPro numOfComboText;
		[SerializeField] private TextMeshPro judgeResultText;
		[SerializeField] private TextMeshPro comboText;

		[SerializeField] private float moveDistance = 0.5f;
		[SerializeField] private float moveDuration = 0.3f;
		[SerializeField] private float fadeDelay = 0.1f;
		[SerializeField] private float fadeDuration = 0.2f;

		public void PlayComboText(int numOfCombo, NoteJudgementResult judgeResult)
		{
			numOfComboText.text = numOfCombo.ToString("D3");
			judgeResultText.text = GetJudgeText(judgeResult);

			SetAlpha(numOfComboText, 1f);
			SetAlpha(judgeResultText, 1f);
			SetAlpha(comboText, 1f);

			var targetPosition = transform.localPosition + Vector3.up * moveDistance;

			var sequence = DOTween.Sequence();

			sequence.Append(
				transform.DOLocalMove(targetPosition, 0.3f)
					.SetEase(Ease.OutQuad));

			sequence.Append(CreateFadeTween(numOfComboText, 0.2f));
			sequence.Join(CreateFadeTween(judgeResultText, 0.2f));
			sequence.Join(CreateFadeTween(comboText, 0.2f));

			sequence.OnComplete(() => Destroy(gameObject));
		}

		private static Tween CreateFadeTween(TMP_Text text, float duration)
		{
			return DOTween.To(
				() => text.color.a,
				alpha => SetAlpha(text, alpha),
				0f,
				duration);
		}

		private static void SetAlpha(TMP_Text text, float alpha)
		{
			var color = text.color;
			color.a = alpha;
			text.color = color;
		}

		private static string GetJudgeText(NoteJudgementResult judgeResult)
		{
			return judgeResult switch
			{
				NoteJudgementResult.Perfect => "PERFECT",
				NoteJudgementResult.Great => "GREAT",
				NoteJudgementResult.Good => "GOOD",
				NoteJudgementResult.Bad => "BAD",
				NoteJudgementResult.Miss => "MISS",
				_ => throw new ArgumentOutOfRangeException(nameof(judgeResult), judgeResult, null)
			};
		}
	}
}
