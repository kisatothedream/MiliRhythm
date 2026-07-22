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
		public static int GetLane(this NoteType type)
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

		public static NoteType GetNoteTypeByLane(int lane)
		{
			return lane switch
			{
				0 => NoteType.Left,
				1 => NoteType.Up,
				2 => NoteType.Down,
				3 => NoteType.Right,
			};
		}
	}

	public class Note : MonoBehaviour
	{
		public bool IsAlive = true;
		public NoteType Type;
		public double StartTime;
		public double JudgeTime;
		public double EndTime;
		public float LaneLength;

		public void UpdateNote(double time)
		{
			if (EndTime < time)
			{
				IsAlive = false;
				gameObject.SetActive(false);
				return;
			}
			var rate = (time - StartTime) / (JudgeTime - StartTime);
			UpdatePosition((float)rate * LaneLength);
		}

		public void UpdatePosition(float y)
		{
			var pos = transform.localPosition;
			pos.y = y;
			transform.localPosition = pos;
		}
	}
}
