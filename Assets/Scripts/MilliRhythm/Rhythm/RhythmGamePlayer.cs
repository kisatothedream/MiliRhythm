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


		private void Awake()
		{
			clock = new();
		}

		public void PlayFromStart()
		{
			var startTime = AudioSettings.dspTime + 3f;
			var endTime = AudioSettings.dspTime + 3f + chart.AudioClip.length + 5;
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
				await UniTask.NextFrame();
			}
		}

		private void TrySpawnNotes()
		{
			for (var i = nextNoteIndex; i < notes.Count; i++)
			{
				if (chart.TickToTime(notes[nextNoteIndex].Tick) < clock.SongTime - ApproachingTime)
				{
					nextNoteIndex++;
					Spawn(notes[nextNoteIndex]);
				}
				else
				{
					break;
				}
			}
		}

		private void Spawn(RhythmNote note)
		{
			var noteInstance = Instantiate(notePrefabs[note.Lane], lanes[note.Lane].localPosition, Quaternion.identity, transform);
		}
	}
}
