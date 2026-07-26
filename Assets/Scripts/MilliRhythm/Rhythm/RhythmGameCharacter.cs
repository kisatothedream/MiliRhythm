using System;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	public class RhythmGameCharacter : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private Sprite[] sprites;

		private Direction currentDirection;
		private float elapsedTime;
		private int indexOfImage;

		private void Start()
		{
			currentDirection = Direction.Idle;
		}

		private void Update()
		{
			UpdateState();
		}

		public void OnLeft(bool pressed)
		{
			if (pressed) ChangeState(Direction.Left);
		}

		public void OnUp(bool pressed)
		{
			if (pressed)
				ChangeState(Direction.Up);
		}

		public void OnDown(bool pressed)
		{
			if (pressed)
				ChangeState(Direction.Down);
		}

		public void OnRight(bool pressed)
		{
			if (pressed)
				ChangeState(Direction.Right);
		}

		public void ChangeState(Direction next)
		{
			currentDirection = next;
			elapsedTime = 0;
		}

		private void UpdateState()
		{
			elapsedTime += Time.deltaTime;
			var imageIndex = currentDirection.GetImageIndex(elapsedTime > 0.3f);
			spriteRenderer.sprite = sprites[imageIndex];

			if (elapsedTime > 1f)
			{
				ChangeState(Direction.Idle);
			}
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
