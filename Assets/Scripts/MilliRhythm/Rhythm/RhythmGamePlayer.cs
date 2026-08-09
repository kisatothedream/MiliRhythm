using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MilliRhythm.Data.Domain;
using MilliRhythm.Data.Repository;
using MilliRhythm.Input;
using MilliRhythm.Scene;
using MilliRhythm.Scene.Contracts;
using MilliRhythm.UI.RhythmGameUI;
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
		[SerializeField] private RhythmGameUIController uiController;
		[SerializeField] private JudgeManager judgeManager;

		private GameControls controls => InputManager.Instance.GameControls;
		private RhythmClock clock;

		private RhythmGameContext context;

		[SerializeField] private bool playPerfect;
		[SerializeField, Range(-1, 1)] private double playerOffset;
		[SerializeField] private AudioSource audioSource;
		[SerializeField] private Note[] notePrefabs;
		[SerializeField] private GameObject[] noteGlows;
		private List<RhythmNote> notes;
		[SerializeField] private Transform[] lanes;
		[SerializeField] private Transform[] laneEnds;
		private const float ApproachingTime = 1f;

		[SerializeField] private Transform startPoint;
		[SerializeField] private Transform endPoint;
		private float laneLength;

		private int nextNoteIndex;
		private Queue<Note>[] waitingNotes = new Queue<Note>[4];
		private List<Note>[] activeNotes = new List<Note>[4];
		private bool[] isLaneHeld = new bool[4];
		private Note[] judgingLongNotes = new Note[4];

		private const double PerfectWindow = 0.045f;
		private const double GreatWindow = 0.09f;
		private const double GoodWindow = 0.135f;
		private const double BadWindow = 0.160f;

		private CancellationTokenSource rhythmGameCts;
		private CompositeDisposable inputDisposable;

		private AsyncOperationHandle<AudioClip> audioClipHandle;

		[SerializeField] private RhythmGameCharacter character;


		private const double LongNoteJudgingInterval = 0.25;

		private readonly Tween[] glowTweens = new Tween[4];

		[SerializeField] private GameObject particlePrefab;

		public void Finish()
		{
			rhythmGameCts.Cancel();
			rhythmGameCts.Dispose();
			rhythmGameCts = null;
			UnregisterInputs();
			if (audioClipHandle.IsValid())
			{
				Addressables.Release(audioClipHandle);
			}

			foreach (var tween in glowTweens)
			{
				tween?.Kill();
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
				EndTime = audioClip.length + 3f,
			};
			return ctx;
		}

		public async UniTask Init(RhythmChart rhythmChart, MusicData musicData)
		{
			uiController.InitializeCurrentTrackContext(RestartGame, Quit);
			for (var i = 0; i < 4; i++)
			{
				var glow = noteGlows[i];
				waitingNotes[i] = new Queue<Note>();
				activeNotes[i] = new List<Note>();
				glowTweens[i] = glow.transform.DOScale(Vector3.one, 0.2f).From(1.4f * Vector3.one)
					.Pause()
					.SetEase(Ease.OutQuad)
					.SetAutoKill(false)
					.OnComplete(() => glow.SetActive(false));
			}


			rhythmGameCts = new CancellationTokenSource();
			judgeManager.Initialize();

			RegisterInputs();
			context = await BuildContext(rhythmChart, musicData);

			clock = new RhythmClock();
			laneLength = Mathf.Abs(startPoint.position.y - endPoint.position.y);

			clock.StartClock(context.StartTime);
			audioSource.clip = context.AudioClip;
			audioSource.PlayScheduled(context.StartTime);
			notes = new List<RhythmNote>(context.Chart.Notes);
			notes.Sort((a, b) => a.Head.CompareTo(b.Head));
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
				if (!clock.IsPaused)
				{
					TrySpawnNotes();
					UpdateNotes();
					uiController.UpdateTrackProgress((float)clock.SongTime / context.AudioClip.length);
				}

				await UniTask.NextFrame(cancellationToken: rhythmGameCts.Token);
			}

			EndGame();
		}

		private void EndGame()
		{
			//Show Result and Retry
			//Return To Music Select Scene
			Debug.Log(
				$"Result Max Combo [{judgeManager.MaxCombo}] - Score [{judgeManager.CurrentScore}] \nPerfect[{judgeManager.PerfectCount}] \nGreat[{judgeManager.GreatCount}] \nGood[{judgeManager.GoodCount}] \nBad[{judgeManager.BadCount}] \nMiss[{judgeManager.MissCount}]");
			var parameter = new MusicSelectorSceneParameter(context.CurrentMusicId, context.CurrentChartType, context.CurrentDifficulty);
			judgeManager.RequestShowResultAndEndGame(context.CurrentMusicId, context.CurrentChartType, parameter);
		}

		private void UpdateNotes()
		{
			for (var lane = 0; lane < 4; lane++)
			{
				TryRemoveExpiredNotes(lane);
				UpdateNotesPosition(lane);
				if (playPerfect)
				{
					PlayNotePefect(lane);
				}

				TryJudgeMissedNotes(lane);
				UpdateLongNotes(lane);
			}
		}

		private void PlayNotePefect(int lane)
		{
			var queue = waitingNotes[lane];
			while (queue.TryPeek(out var note) && clock.SongTime > note.HeadTime)
			{
				OnPressKey(lane);
			}
		}

		private void UpdateNotesPosition(int lane)
		{
			foreach (var note in activeNotes[lane])
			{
				note.UpdateNote(clock.SongTime);
			}
		}

		private void TryRemoveExpiredNotes(int lane)
		{
			for (var i = activeNotes[lane].Count - 1; i >= 0; i--)
			{
				var note = activeNotes[lane][i];
				if (clock.SongTime > note.EndTime)
				{
					activeNotes[lane].Remove(note);
				}
			}
		}

		private void UpdateLongNotes(int lane)
		{
			if (judgingLongNotes[lane] == null) return;
			var note = judgingLongNotes[lane];
			while (clock.SongTime > note.NextJudgeTime)
			{
				if (note.NextJudgeTime < note.HeadTime + note.NoteLength)
				{
					if (isLaneHeld[lane])
					{
						judgeManager.OnHitNote(NoteJudgementResult.Perfect);
						judgeManager.CreateComboText(NoteJudgementResult.Perfect);
						note.NextJudgeTime += LongNoteJudgingInterval;
					}
					else
					{
						judgeManager.OnMissNote();
						judgeManager.CreateComboText(NoteJudgementResult.Miss);
					}
				}
				else
				{
					judgingLongNotes[lane] = null;
					break;
				}
			}
		}

		private void TrySpawnNotes()
		{
			while (nextNoteIndex < notes.Count)
			{
				var note = notes[nextNoteIndex];
				var judgeTime = context.Chart.TickTimeToTime(note.Head) + playerOffset;
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
			noteInstance.NoteLength = context.Chart.TickToTime(note.LengthTick);
			noteInstance.Lane = note.Lane;
			noteInstance.StartTime = judgeTime - ApproachingTime;
			noteInstance.HeadTime = judgeTime;

			noteInstance.EndTime = judgeTime + noteInstance.NoteLength + 1;
			noteInstance.LaneLength = laneLength;

			waitingNotes[note.Lane].Enqueue(noteInstance);
			activeNotes[note.Lane].Add(noteInstance);
		}

		private void TryJudgeMissedNotes(int lane)
		{
			var queue = waitingNotes[lane];
			while (queue.TryPeek(out var note) && clock.SongTime > note.HeadTime + BadWindow)
			{
				queue.Dequeue();
				judgeManager.OnMissNote();
				judgeManager.CreateComboText(NoteJudgementResult.Miss);
			}
			// foreach (var note in activeNotes)
		}

		private void OnPressKey(int lane)
		{
			var queue = waitingNotes[lane];
			PlayGlow(lane);
			if (queue.TryPeek(out var note) && Math.Abs(clock.SongTime - note.HeadTime) < BadWindow)
			{
				var result = JudgeTime(clock.SongTime, note.HeadTime);
				Debug.Log(result);
				character.ChangeState(lane);
				judgeManager.OnHitNote(result);
				judgeManager.CreateComboText(result);
				queue.Dequeue();
				CreateNoteHitParticleAsync(lane).Forget();
				if (note.IsLongNote)
				{
					judgingLongNotes[lane] = note;
					note.NextJudgeTime = note.HeadTime + LongNoteJudgingInterval;
				}
				else
				{
					activeNotes[lane].Remove(note);
					Destroy(note.gameObject);
				}
			}
		}

		private async UniTask CreateNoteHitParticleAsync(int lane)
		{
			var go = Instantiate(particlePrefab, laneEnds[lane]);
			await UniTask.WaitForSeconds(1);
			Destroy(go);
		}

		private NoteJudgementResult JudgeTime(double currentTime, double judgeTime)
		{
			var delta = Mathf.Abs((float)(judgeTime - currentTime));
			if (delta < PerfectWindow) return NoteJudgementResult.Perfect;
			if (delta < GreatWindow) return NoteJudgementResult.Great;
			if (delta < GoodWindow) return NoteJudgementResult.Good;
			if (delta < BadWindow) return NoteJudgementResult.Bad;
			return NoteJudgementResult.NotReached;
		}

		private void PlayGlow(int lane)
		{
			noteGlows[lane].SetActive(true);
			glowTweens[lane].Restart();
		}

		private void RestartGame()
		{
			SceneController.Instance.RequestChangeScene(new RhythmGameSceneParameter(context.CurrentMusicId, context.CurrentChartType,
				context.CurrentDifficulty));
		}

		private void Quit()
		{
			SceneController.Instance.RequestChangeScene(new MusicSelectorSceneParameter()
			{
				LastMusicId = context.CurrentMusicId,
				LastChartType = context.CurrentChartType,
				LastDifficulty = context.CurrentDifficulty,
			});
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
					$"Preview_{rhythmNote.Head}_{rhythmNote.Lane}";

				noteInstance.gameObject.hideFlags =
					HideFlags.HideAndDontSave;

				var judgeTime =
					previewChart.TickTimeToTime(rhythmNote.Head);

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
