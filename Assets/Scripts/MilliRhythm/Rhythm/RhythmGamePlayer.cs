using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.Common;
using MilliRhythm.Input;
using R3;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	public partial class RhythmGamePlayer : MonoBehaviour
	{
		private GameControls controls => InputManager.Instance.GameControls;
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

		private const double PerfectRangeTime = 0.045f;
		private const double GoodRangeTime = 0.09f;
		private const double NormalRangeTime = 0.135f;
		private const double BadRangeTime = 0.160f;

		private CompositeDisposable disposable;

		private void Awake()
		{
			clock = new();
		}

		private void Register()
		{
			disposable = new CompositeDisposable();
			controls.Left.Subscribe(OnLeft).AddTo(disposable);
			controls.Up.Subscribe(OnUp).AddTo(disposable);
			controls.Down.Subscribe(OnDown).AddTo(disposable);
			controls.Right.Subscribe(OnRight).AddTo(disposable);
		}

		private void Unregister()
		{
			disposable.Dispose();
		}

		public void PlayFromStart()
		{
			Register();

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
			for (var i = activeNotes.Count - 1; i >= 0; i--)
			{
				var note = activeNotes[i];

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
			var length = chart.TickToTime(note.LengthTick);
			noteInstance.Lane = note.Lane;
			noteInstance.NoteLength = length;
			noteInstance.StartTime = judgeTime - ApproachingTime;
			noteInstance.HeadTime = judgeTime;
			noteInstance.TailTime = judgeTime + length;
			noteInstance.EndTime = judgeTime + length + 1;
			noteInstance.LaneLength = laneLength;
			activeNotes.Add(noteInstance);
		}

		private void OnLeft(bool pressed)
		{
			if (pressed) JudgeNotesDown(NoteType.Left.GetLane());
			else JudgeNotesUp(NoteType.Left.GetLane());
		}

		private void OnUp(bool pressed)
		{
			if (pressed) JudgeNotesDown(NoteType.Up.GetLane());
			else JudgeNotesUp(NoteType.Up.GetLane());
		}

		private void OnDown(bool pressed)
		{
			if (pressed) JudgeNotesDown(NoteType.Down.GetLane());
			else JudgeNotesUp(NoteType.Down.GetLane());
		}

		private void OnRight(bool pressed)
		{
			if (pressed) JudgeNotesDown(NoteType.Right.GetLane());
			else JudgeNotesUp(NoteType.Right.GetLane());
		}

		private void JudgeNotesDown(int lane)
		{
			Debug.Log($"Judge Down");
			for (var i = 0; i < activeNotes.Count; i++)
			{
				var note = activeNotes[i];
				if (note.IsJudgedDown) continue;
				if (note.Lane != lane) continue;

				var current = clock.SongTime;
				var result = JudgeTime(current, note.HeadTime);
				if (result != NoteJudgementResult.NotReached)
				{
					note.JudgeDown(result);
					return;
				}
			}
		}

		private void JudgeNotesUp(int lane)
		{
			Debug.Log($"Judge Up");
			for (var i = 0; i < activeNotes.Count; i++)
			{
				var note = activeNotes[i];
				if (!note.IsLongNote) continue;
				if (!note.IsJudgedDown) continue;
				if (note.IsJudgedUp) continue;
				if (note.Lane != lane) continue;

				var current = clock.SongTime;
				var result = JudgeTime(current, note.TailTime);
				if (result != NoteJudgementResult.NotReached)
				{
					note.JudgeUp(result);
					return;
				}
			}
		}

		private NoteJudgementResult JudgeTime(double currentTime, double judgeTime)
		{
			var delta = Mathf.Abs((float)(judgeTime - currentTime));
			if (delta < PerfectRangeTime) return NoteJudgementResult.Perfect;
			if (delta < GoodRangeTime) return NoteJudgementResult.Good;
			if (delta < NormalRangeTime) return NoteJudgementResult.Normal;
			if (delta < BadRangeTime) return NoteJudgementResult.Bad;
			return NoteJudgementResult.NotReached;
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

				noteInstance.Lane = rhythmNote.Lane;

				noteInstance.StartTime =
					judgeTime - ApproachingTime;

				noteInstance.HeadTime =
					judgeTime;

				noteInstance.EndTime =
					judgeTime + 1.0;

				noteInstance.LaneLength =
					laneLength;

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
