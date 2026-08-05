using System;
using System.Collections.Generic;
using MilliRhythm.Data.Domain;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	[Serializable]
	[CreateAssetMenu(fileName = "RhythmChart", menuName = "Rhythm Game/Rhythm Chart")]
	public class RhythmChart : ScriptableObject
	{
		[field: SerializeField] public int MusicId;
		[field: SerializeField] public ChartType ChartType;
		[field: SerializeField] public Difficulty Difficulty;

		[SerializeField, HideInInspector] private double bpm;
		[SerializeField, HideInInspector] private double offsetSeconds;
		[SerializeField] private List<RhythmNote> notes = new();
		private const int ticksPerBeat = 480;
		private const int laneCount = 4;

#if UNITY_EDITOR
		[SerializeField] private AudioClip editorAudioClip;
		public AudioClip EditorAudioClip => editorAudioClip;
#endif
		public double Bpm => bpm;
		public double OffsetSeconds => offsetSeconds;
		public int TicksPerBeat => ticksPerBeat;
		public int LaneCount => laneCount;
		public List<RhythmNote> Notes => notes;

		public void SetBpm(double value)
		{
			bpm = Math.Max(1f, value);
		}

		public void SetOffsetSeconds(double value)
		{
			offsetSeconds = value;
		}

		public double TickToBeat(int tick)
		{
			return (double)tick / ticksPerBeat;
		}

		public int BeatToTick(double beat)
		{
			return (int)Math.Round(beat * ticksPerBeat);
		}

		public double TickToTime(RhythmNote note)
		{
			return TickToTime(note.Tick);
		}

		public double TickToTime(int tick)
		{
			var beat = TickToBeat(tick);
			return offsetSeconds + beat * 60f / bpm;
		}

		public int TimeToTick(double time)
		{
			var beat = (time - offsetSeconds) * bpm / 60f;
			return BeatToTick(beat);
		}

		private void OnValidate()
		{
			bpm = Math.Max(1f, bpm);
		}
	}

	[Serializable]
	public class RhythmNote
	{
		public int Tick;
		public int Lane;
		public int LengthTick;

		public override string ToString()
		{
			return $"{Tick}:{Lane}";
		}
	}
}
