using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	public class RhythmGameCharacter : MonoBehaviour
	{
		public bool IsPaused;
		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private Sprite[] idleSprites;
		[SerializeField] private Sprite[] leftSprites;
		[SerializeField] private Sprite[] rightSprites;
		[SerializeField] private Sprite[] upSprites;
		[SerializeField] private Sprite[] downSprites;
		private Sprite[] currentSpriteSet;

		private float elapsedTime;
		private int indexOfImage;

		[SerializeField, Range(0.01f, 1f)] private float indexUpdateTime;
		[SerializeField, Range(0.01f, 3f)] private float returnToIdleTime;
		[SerializeField, Range(0.01f, 1f)] private float changeSizeTime;
		[SerializeField, Range(0.001f, 0.05f)] private float changeSizeRate;

		private CancellationTokenSource updateIndexCts = new();
		private CancellationTokenSource sizeModifierCts;
		private CancellationTokenSource returnToIdleCts = new();

		private void Start()
		{
			OnIdle();
			UpdateIndex().Forget();
		}

		private void OnDestroy()
		{
			updateIndexCts?.Cancel();
			updateIndexCts?.Dispose();
			updateIndexCts = null;

			sizeModifierCts?.Cancel();
			sizeModifierCts?.Dispose();
			sizeModifierCts = null;

			returnToIdleCts?.Cancel();
			returnToIdleCts?.Dispose();
			returnToIdleCts = null;
		}

		private async UniTask UpdateIndex()
		{
			while (updateIndexCts.Token.CanBeCanceled)
			{
				if (!IsPaused)
				{
					indexOfImage = (indexOfImage + 1) % 3;
					spriteRenderer.sprite = currentSpriteSet[indexOfImage];
				}

				await UniTask.WaitForSeconds(indexUpdateTime, cancellationToken: updateIndexCts.Token);
			}
		}

		public void ChangeState(int lane)
		{
			returnToIdleCts?.Cancel();
			returnToIdleCts?.Dispose();
			returnToIdleCts = new CancellationTokenSource();
			switch (lane)
			{
				case 0:
					OnLeft().Forget();
					break;
				case 1:
					OnUp().Forget();
					break;
				case 2:
					OnDown().Forget();
					break;
				case 3:
					OnRight().Forget();
					break;
			}

			ChangeSizeAsync().Forget();
		}

		private void OnIdle()
		{
			currentSpriteSet = idleSprites;
			spriteRenderer.sprite = currentSpriteSet[0];
		}

		private async UniTask OnLeft()
		{
			currentSpriteSet = leftSprites;
			spriteRenderer.sprite = currentSpriteSet[0];
			await UniTask.WaitForSeconds(returnToIdleTime, cancellationToken: returnToIdleCts.Token);
			OnIdle();
		}

		private async UniTask OnUp()
		{
			currentSpriteSet = upSprites;
			spriteRenderer.sprite = currentSpriteSet[0];
			await UniTask.WaitForSeconds(returnToIdleTime, cancellationToken: returnToIdleCts.Token);
			OnIdle();
		}

		private async UniTask OnDown()
		{
			currentSpriteSet = downSprites;
			spriteRenderer.sprite = currentSpriteSet[0];
			await UniTask.WaitForSeconds(returnToIdleTime, cancellationToken: returnToIdleCts.Token);
			OnIdle();
		}

		private async UniTask OnRight()
		{
			currentSpriteSet = rightSprites;
			spriteRenderer.sprite = currentSpriteSet[0];
			await UniTask.WaitForSeconds(returnToIdleTime, cancellationToken: returnToIdleCts.Token);
			OnIdle();
		}

		private async UniTask ChangeSizeAsync()
		{
			sizeModifierCts?.Cancel();
			sizeModifierCts?.Dispose();
			sizeModifierCts = new CancellationTokenSource();
			transform.localScale = new Vector2(1 - changeSizeRate, 1 + changeSizeRate);
			await UniTask.WaitForSeconds(changeSizeTime, cancellationToken: sizeModifierCts.Token);
			transform.localScale = Vector2.one;
		}
	}

	public enum Direction
	{
		Idle,
		Left,
		Up,
		Down,
		Right,
	}

	public static class DirectionExtensions
	{
		public static int GetImageIndex(this Direction direction, bool next)
		{
			return (direction, next) switch
			{
				(Direction.Idle, _) => 0,
				(Direction.Left, false) => 1,
				(Direction.Left, true) => 2,
				(Direction.Up, false) => 3,
				(Direction.Up, true) => 4,
				(Direction.Down, false) => 5,
				(Direction.Down, true) => 6,
				(Direction.Right, false) => 7,
				(Direction.Right, true) => 8,
			};
		}
	}
}
