using System;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	public enum NoteType
	{
		Left,
		Up,
		Down,
		Right,
	}

	public static class NoteTypeExtensions
	{
		public static int ToInt(this NoteType type)
		{
			return type switch
			{
				NoteType.Left => 0,
				NoteType.Up => 1,
				NoteType.Down => 2,
				NoteType.Right => 3,
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			};
		}

		public static NoteType ToLane(this int lane)
		{
			return lane switch
			{
				0 => NoteType.Left,
				1 => NoteType.Up,
				2 => NoteType.Down,
				3 => NoteType.Right,
				_ => throw new ArgumentOutOfRangeException(nameof(lane), lane, null)
			};
		}
	}

	public class Note : MonoBehaviour
	{
		public Vector3 Position
		{
			get => transform.position;
			set => transform.position = value;
		}
	}
}
