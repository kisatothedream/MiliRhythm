using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.Common;
using MilliRhythm.Data.Repository;
using MilliRhythm.Input;
using MilliRhythm.Scene;
using MilliRhythm.Scene.Contracts;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MilliRhythm.Rhythm
{
	public struct RhythmGameContext
	{
		public int CurrentMusicId;
		public ChartType CurrentChartType;
		public Difficulty CurrentDifficulty;

		public AudioClip AudioClip;
		public RhythmChart Chart;
		public double StartTime;
		public double EndTime;
	}

	public enum RhythmGameModifier
	{
	}

	public partial class RhythmGamePlayer : MonoBehaviour
	{
		private GameControls controls => InputManager.Instance.GameControls;
		private RhythmClock clock;

		private RhythmGameContext context;
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

		private const double PerfectRangeTime = 0.045f;
		private const double GoodRangeTime = 0.09f;
		private const double NormalRangeTime = 0.135f;
		private const double BadRangeTime = 0.160f;

		private CompositeDisposable inputDisposable;
		private CompositeDisposable characterDisposable;

		private AsyncOperationHandle<AudioClip> audioClipHandle;

		[SerializeField] private RhythmGameCharacter character;

		public void Finish()
		{
			UnregisterInputs();
			UnregisterCharacter();
			if (audioClipHandle.IsValid())
			{
				Addressables.Release(audioClipHandle);
			}
		}

		private async UniTask<RhythmGameContext> BuildContext(RhythmChart rhythmChart, MusicData musicData)
		{
			audioClipHandle = Addressables.LoadAssetAsync<AudioClip>(musicData.AudioClipReference);
			var audioClip = await audioClipHandle.Task;
			var ctx = new RhythmGameContext
			{
				CurrentMusicId = musicData.Id,
				CurrentChartType = rhythmChart.ChartType,
				CurrentDifficulty = rhythmChart.Difficulty,
				Chart = rhythmChart,
				AudioClip = audioClip,
				StartTime = AudioSettings.dspTime + 1f,
				EndTime = audioClip.length + 5f,
			};
			return ctx;
		}

		public async UniTask InitializeGamePlayer(RhythmChart rhythmChart, MusicData musicData)
		{
			RegisterInputs();
			RegisterCharacter();
			context = await BuildContext(rhythmChart, musicData);

			clock = new RhythmClock();
			laneLength = Mathf.Abs(startPoint.position.y - endPoint.position.y);

			clock.StartClock(context.StartTime);
			audioSource.clip = context.AudioClip;
			audioSource.PlayScheduled(context.StartTime);
			notes = new List<RhythmNote>(context.Chart.Notes);
			notes.Sort((a, b) => a.Tick.CompareTo(b.Tick));
		}

		public void StartGame()
		{
			PlaySong(context.EndTime).Forget();
		}

		private async UniTask PlaySong(double endTime)
		{
			while (clock.SongTime < endTime)
			{
				//Paused
				TrySpawnNotes();
				UpdateNotes();
				await UniTask.NextFrame();
			}

			EndGame();
		}

		private void EndGame()
		{
			//Show Result and Retry
			//Return To Music Select Scene
			SceneController.Instance.RequestChangeScene(new MusicSelectorSceneParameter(context.CurrentMusicId, context.CurrentChartType,
				context.CurrentDifficulty));
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
				var judgeTime = context.Chart.TickToTime(note);
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
			var noteLength = context.Chart.TickToTime(note);
			noteInstance.Lane = note.Lane;
			noteInstance.NoteLength = noteLength;
			noteInstance.StartTime = judgeTime - ApproachingTime;
			noteInstance.HeadTime = judgeTime;
			noteInstance.TailTime = judgeTime + noteLength;
			noteInstance.EndTime = judgeTime + noteLength + 1;
			noteInstance.LaneLength = laneLength;
			activeNotes.Add(noteInstance);
		}

		private void JudgeNotesDown(int lane)
		{
			foreach (var note in activeNotes)
			{
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
			foreach (var note in activeNotes)
			{
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

			foreach (var rhythmNote in previewChart.Notes)
			{
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
			foreach (var note in editorPreviewNotes)
			{
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

			foreach (var laneTransform in lanes)
			{
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
