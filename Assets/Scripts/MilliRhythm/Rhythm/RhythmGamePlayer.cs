using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	public class RhythmGamePlayer : MonoBehaviour
	{
		private RhythmClock clock;
		[SerializeField] private RhythmChart chart;
		[SerializeField] private AudioSource audioSource;
		[SerializeField] private Note[] notePrefabs;
		private List<RhythmNote> notes;
		[SerializeField] private Transform[] lanes;
		private const float ApproachingTime = 2f;
		private int nextNoteIndex;
		private List<Note> activeNotes = new();

		[SerializeField] private Transform startPoint;
		[SerializeField] private Transform endPoint;
		private float laneLength;
		private float speed;


		private void Awake()
		{
			clock = new();
		}

		public void PlayFromStart()
		{
			nextNoteIndex = 0;
			laneLength = Mathf.Abs(startPoint.position.y - endPoint.position.y);
			speed = laneLength / ApproachingTime;

			var startTime = AudioSettings.dspTime + 1f;
			var endTime = chart.AudioClip.length + 5f;
			activeNotes.Clear();
			clock.StartClock(startTime);
			audioSource.clip = chart.AudioClip;
			audioSource.PlayScheduled(startTime);
			notes = new List<RhythmNote>(chart.Notes);
			notes.Sort((a, b) => a.Tick.CompareTo(b.Tick));
			PlaySong(endTime).Forget();
		}

		private async UniTask PlaySong(double endTime)
		{
			while (clock.SongTime < endTime)
			{
				TrySpawnNotes();
				UpdateNotes();
				await UniTask.NextFrame();
			}
		}

		private void UpdateNotes()
		{
			foreach (var note in activeNotes)
			{
				note.UpdateNote(clock.SongTime);
			}
		}

		private void TrySpawnNotes()
		{
			while (nextNoteIndex < notes.Count)
			{
				var note = notes[nextNoteIndex];
				var judgeTime = chart.TickToTime(note);
				var spawnTime = judgeTime - ApproachingTime;

				if (spawnTime > clock.SongTime)
					break;

				Spawn(note, judgeTime);
				nextNoteIndex++;
			}
		}

		private void Spawn(RhythmNote note, double judgeTime)
		{
			var noteInstance = Instantiate(notePrefabs[note.Lane], lanes[note.Lane].localPosition, Quaternion.identity, lanes[note.Lane]);
			noteInstance.StartTime = judgeTime - ApproachingTime;
			noteInstance.JudgeTime = judgeTime;
			noteInstance.EndTime = judgeTime + 1;
			noteInstance.LaneLength = laneLength;
			activeNotes.Add(noteInstance);
		}
	}
}
