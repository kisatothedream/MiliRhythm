
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MilliRhythm.Data.Domain;
using MilliRhythm.Input;
using MilliRhythm.UI.RhythmGameUI;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MilliRhythm.Rhythm
{
	public partial class RhythmGamePlayer : MonoBehaviour
	{
		[SerializeField] private RhythmGameUIController uiController;
		[SerializeField] private JudgeManager judgeManager;

		private GameControls controls => InputManager.Instance.GameControls;

		[SerializeField] private bool playPerfect;
		[SerializeField, Range(-1, 1)] private double playerOffset;
		[SerializeField] private AudioSource audioSource;
		[SerializeField] private Note[] notePrefabs;
		[SerializeField] private GameObject[] noteGlows;
		[SerializeField] private Transform[] lanes;
		[SerializeField] private Transform[] laneEnds;

		[SerializeField] private Transform startPoint;
		[SerializeField] private Transform endPoint;

		private int nextNoteIndex;
		private Queue<Note>[] waitingNotes = new Queue<Note>[4];
		private List<Note>[] activeNotes = new List<Note>[4];

		private const float PerfectWindow = 0.045f;
		private const float GreatWindow = 0.09f;
		private const float GoodWindow = 0.135f;
		private const float BadWindow = 0.160f;

		private CancellationTokenSource rhythmGameCts;
		private CompositeDisposable inputDisposable;

		private AsyncOperationHandle<AudioClip> audioClipHandle;

		[SerializeField] private RhythmGameCharacter character;

		private readonly Tween[] glowTweens = new Tween[4];

		[SerializeField] private GameObject[] particlePrefabs;

		private int hitNotesCount;
		private double errorSum;

		private bool isPaused;

		private float baseSpeed;

		public void Finish()
		{
			rhythmGameCts?.Cancel();
			rhythmGameCts?.Dispose();
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

		public async UniTask Init()
		{
			baseSpeed = 1;
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

			// audioSource.clip = context.AudioClip;
		}

		public void StartGame()
		{
			PlaySong().Forget();
		}

		private async UniTask PlaySong()
		{
			while (rhythmGameCts.Token.CanBeCanceled)
			{
				//Paused
				if (!isPaused)
				{
					TrySpawnNotes();
					UpdateNotes();
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
			// judgeManager.RequestShowResultAndEndGame(context.CurrentMusicId, context.CurrentChartType, 1000 * errorSum / hitNotesCount, parameter);
		}

		private void UpdateNotes()
		{
			for (var lane = 0; lane < 4; lane++)
			{
				UpdateNotesPosition(lane);
				if (playPerfect)
				{
					PlayNotePerfect(lane);
				}

				TryJudgeMissedNotes(lane);
			}
		}

		private void PlayNotePerfect(int lane)
		{
		}

		private void UpdateNotesPosition(int lane)
		{
			foreach (var note in activeNotes[lane])
			{
				note.Position += (laneEnds[lane].position - lanes[lane].position) * (baseSpeed * Time.deltaTime);
			}
		}

		private void TrySpawnNotes()
		{
			//SpawnTimer
			// Spawn(note, judgeTime);
		}

		private void Spawn(int lane)
		{
			var noteInstance = Instantiate(notePrefabs[lane], lanes[lane].localPosition, Quaternion.identity, lanes[lane]);
			waitingNotes[lane].Enqueue(noteInstance);
			activeNotes[lane].Add(noteInstance);
		}

		private void TryJudgeMissedNotes(int lane)
		{
			var queue = waitingNotes[lane];
			while (queue.TryPeek(out var note))
			{
				var signedDistance = Vector3.Dot(
					note.transform.position - laneEnds[lane].position,
					(laneEnds[lane].position - note.transform.position).normalized
				);
				if (signedDistance > BadWindow)
				{
					queue.Dequeue();
					judgeManager.OnMissNote();
					judgeManager.CreateComboText(NoteJudgementResult.Miss);

					activeNotes[lane].Remove(note);
				}
			}
		}

		private void OnPressKey(int lane)
		{
			if (isPaused) return;
			var queue = waitingNotes[lane];
			PlayGlow(lane);
			if (queue.TryPeek(out var note) && Vector3.Distance(note.Position, laneEnds[lane].position) <= BadWindow)
			{
				var result = JudgeTime(note.Position, laneEnds[lane].position);
				// Debug.Log(result);
				// CompareNoteTiming(result, judgeTime, note.HeadTime);
				character.ChangeState(lane);
				judgeManager.OnHitNote(result);
				judgeManager.CreateComboText(result);
				queue.Dequeue();
				CreateNoteHitParticleAsync(lane).Forget();
				activeNotes[lane].Remove(note);
				Destroy(note.gameObject);
			}
		}

		private async UniTask CreateNoteHitParticleAsync(int lane)
		{
			var go = Instantiate(particlePrefabs[lane], laneEnds[lane]);
			await UniTask.WaitForSeconds(1);
			Destroy(go);
		}

		private NoteJudgementResult JudgeTime(Vector3 notePosition, Vector3 endPosition)
		{
			var delta = Vector3.Distance(notePosition, endPosition);
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
		}

		private void Quit()
		{
		}

		private void PauseGame()
		{
			isPaused = true;
			audioSource.Pause();
			character.IsPaused = true;
		}

		private void ResumeGame()
		{
			isPaused = false;
			audioSource.Play();
			character.IsPaused = false;
		}
	}
}
