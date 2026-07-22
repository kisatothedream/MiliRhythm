using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	public partial class RhythmGamePlayer : MonoBehaviour
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
			for (int i = activeNotes.Count - 1; i >= 0; i--)
			{
				Note note = activeNotes[i];

				note.UpdateNote(clock.SongTime);

				if (clock.SongTime < note.EndTime)
					continue;

				activeNotes.RemoveAt(i);
				Destroy(note.gameObject);
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

	public partial class RhythmGamePlayer
	{
#if UNITY_EDITOR
		private const string EditorPreviewRootName = "__RhythmEditorPreview__";

		[NonSerialized] private readonly List<Note> editorPreviewNotes = new();

		[NonSerialized] private readonly List<Transform> editorPreviewRoots = new();

		public void CreateEditorPreview(RhythmChart previewChart)
		{
			DestroyEditorPreview();

			if (previewChart == null)
			{
				Debug.LogWarning("프리뷰할 RhythmChart가 없습니다.");
				return;
			}

			if (startPoint == null || endPoint == null)
			{
				Debug.LogWarning("RhythmGamePlayer의 StartPoint와 EndPoint가 필요합니다.");
				return;
			}

			laneLength = Mathf.Abs(
				startPoint.position.y - endPoint.position.y);

			CreateEditorPreviewRoots();

			for (var i = 0; i < previewChart.Notes.Count; i++)
			{
				var rhythmNote = previewChart.Notes[i];

				if (!CanCreateEditorPreviewNote(rhythmNote.Lane))
				{
					continue;
				}

				var root = editorPreviewRoots[rhythmNote.Lane];

				var noteInstance = Instantiate(
					notePrefabs[rhythmNote.Lane],
					root,
					false);

				noteInstance.name =
					$"Preview_{rhythmNote.Tick}_{rhythmNote.Lane}";

				noteInstance.gameObject.hideFlags =
					HideFlags.HideAndDontSave;

				var judgeTime =
					previewChart.TickToTime(rhythmNote.Tick);

				noteInstance.Type =
					NoteTypeExtensions.GetNoteTypeByLane(
						rhythmNote.Lane);

				noteInstance.StartTime =
					judgeTime - ApproachingTime;

				noteInstance.JudgeTime =
					judgeTime;

				noteInstance.EndTime =
					judgeTime + 1.0;

				noteInstance.LaneLength =
					laneLength;

				noteInstance.IsAlive = true;
				noteInstance.gameObject.SetActive(false);

				editorPreviewNotes.Add(noteInstance);
			}
		}

		public void UpdateEditorPreview(double songTime)
		{
			for (var i = 0; i < editorPreviewNotes.Count; i++)
			{
				var note = editorPreviewNotes[i];

				if (note == null)
				{
					continue;
				}

				var isVisible =
					songTime >= note.StartTime &&
					songTime <= note.EndTime;

				if (note.gameObject.activeSelf != isVisible)
				{
					note.gameObject.SetActive(isVisible);
				}

				if (!isVisible)
				{
					continue;
				}

				note.IsAlive = true;
				note.UpdateNote(songTime);
			}
		}

		public void DestroyEditorPreview()
		{
			editorPreviewNotes.Clear();
			editorPreviewRoots.Clear();

			if (lanes == null)
			{
				return;
			}

			for (var lane = 0; lane < lanes.Length; lane++)
			{
				var laneTransform = lanes[lane];

				if (laneTransform == null)
				{
					continue;
				}

				for (var i = laneTransform.childCount - 1;
				     i >= 0;
				     i--)
				{
					var child = laneTransform.GetChild(i);

					if (!child.name.StartsWith(
						    EditorPreviewRootName,
						    StringComparison.Ordinal))
					{
						continue;
					}

					DestroyPreviewObject(child.gameObject);
				}
			}
		}

		private void CreateEditorPreviewRoots()
		{
			editorPreviewRoots.Clear();

			for (var lane = 0; lane < lanes.Length; lane++)
			{
				var rootObject = new GameObject($"{EditorPreviewRootName}_{lane}");

				rootObject.hideFlags = HideFlags.HideAndDontSave;

				var root = rootObject.transform;

				root.SetParent(lanes[lane], false);
				root.localPosition = Vector3.zero;
				root.localRotation = Quaternion.identity;
				root.localScale = Vector3.one;

				editorPreviewRoots.Add(root);
			}
		}

		private bool CanCreateEditorPreviewNote(int lane)
		{
			if (lane < 0 || lane >= lanes.Length)
			{
				return false;
			}

			if (lane >= notePrefabs.Length)
			{
				return false;
			}

			return lanes[lane] != null && notePrefabs[lane] != null;
		}

		private static void DestroyPreviewObject(GameObject previewObject)
		{
			if (previewObject == null)
			{
				return;
			}

			if (Application.isPlaying)
			{
				Destroy(previewObject);
			}
			else
			{
				DestroyImmediate(previewObject);
			}
		}
#endif
	}
}
