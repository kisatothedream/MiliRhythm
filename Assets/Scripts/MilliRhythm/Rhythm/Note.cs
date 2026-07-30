using System;
using MilliRhythm.Data.Domain;
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
				_ => throw new ArgumentOutOfRangeException(nameof(lane), lane, null)
			};
		}
	}

	public class Note : MonoBehaviour
	{
		public bool IsLongNote => NoteLength > 0;
		public bool IsJudgedDown;
		public bool IsJudgedUp;
		public int Lane;
		public double NoteLength;
		public double StartTime;
		public double HeadTime;
		public double TailTime;
		public double EndTime;
		public float LaneLength;

		public void UpdateNote(double time)
		{
			var rate = (time - StartTime) / (HeadTime - StartTime);
			UpdatePosition((float)rate * LaneLength);
		}

		private void UpdatePosition(float y)
		{
			var pos = transform.localPosition;
			pos.y = y;
			transform.localPosition = pos;
		}

		public void JudgeDown(NoteJudgementResult result)
		{
			Debug.Log($"Head : {result}");
			IsJudgedDown = true;
		}

		public void JudgeUp(NoteJudgementResult result)
		{
			Debug.Log($"LongNoteTail : {result}");
			IsJudgedUp = true;
		}
	}
}
