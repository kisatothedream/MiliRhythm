using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MilliRhythm.Data.Domain;
using MilliRhythm.Input;
using MilliRhythm.Scene;
using MilliRhythm.Scene.Contracts;
using MilliRhythm.UI.RhythmGameUI;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace MilliRhythm.Rhythm
{
	public partial class RhythmGamePlayer : MonoBehaviour
	{
		[SerializeField] private RhythmGameUIController uiController;

		private GameControls controls => InputManager.Instance.GameControls;

		[SerializeField] private ComboText comboTextPrefab;
		[SerializeField] private Transform comboTextPivot;

		[SerializeField] private bool playPerfect;
		[SerializeField] private AudioSource audioSource;
		[SerializeField] private Note[] notePrefabs;
		[SerializeField] private GameObject[] noteGlows;
		[SerializeField] private Transform[] lanes;
		[SerializeField] private Transform[] laneEnds;

		[SerializeField] private Transform startPoint;
		[SerializeField] private Transform endPoint;

		[SerializeField, Range(0, 2)] private float musicStartDelay;

		private int nextNoteIndex;
		private Queue<Note>[] waitingNotes = new Queue<Note>[4];
		private List<Note>[] activeNotes = new List<Note>[4];

		private const float PerfectWindow = 0.25f;
		private const float GreatWindow = 0.4f;
		private const float GoodWindow = 1f;
		private const float BadWindow = 1.2f;

		public int MaxCombo;
		public int AccumNotes;
		public int CurrentCombo;
		public int CurrentScore;
		public int PerfectCount;
		public int GreatCount;
		public int GoodCount;
		public int BadCount;
		public int MissCount;

		private bool isGameStarted;
		private bool isGameOver;

		private CancellationTokenSource rhythmGameCts;
		private CompositeDisposable inputDisposable;

		private AsyncOperationHandle<AudioClip> audioClipHandle;

		[SerializeField] private RhythmGameCharacter character;

		private readonly Tween[] glowTweens = new Tween[4];

		[SerializeField] private GameObject[] particlePrefabs;

		private int hitNotesCount;
		private double errorSum;

		private bool isPaused;

		private int level;
		private float currentSpeed;
		private float timer;
		public const int MaxLife = 5;

		public int RemainLife
		{
			get => remainLife;
			set
			{
				remainLife = Math.Clamp(value, 0, MaxLife);
				UpdateLife(remainLife);
			}
		}

		private int remainLife;

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
			uiController.SetResultActions(PauseGame, ResumeGame, RestartGame, Quit);
			currentSpeed = 1;
			RemainLife = 5;
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

			uiController.SetGameStartAction(StartGame);
			RegisterInputs();
		}

		public void OnStart()
		{
			uiController.ShowGameStartPopup();
		}

		private void StartGame()
		{
			isGameStarted = true;
			PlaySong().Forget();
			uiController.HideGameStartPopup();
		}

		private async UniTaskVoid PlaySong()
		{
			audioSource.PlayDelayed(0.7f);
			while (!rhythmGameCts.Token.IsCancellationRequested)
			{
				//Paused
				if (!isPaused)
				{
					timer -= currentSpeed * Time.deltaTime;
					TrySpawnNotes();
					UpdateNotes();
				}

				await UniTask.NextFrame(cancellationToken: rhythmGameCts.Token);
			}
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
				note.Position += (laneEnds[lane].position - lanes[lane].position) * (currentSpeed * Time.deltaTime);
			}
		}

		private void TrySpawnNotes()
		{
			var rand = Random.Range(0, 100);

			if (timer < 0)
			{
				Spawn(Random.Range(0, 4));
				var delta = 1f / (150f / 60f);
				if (rand < 5 * level)
				{
					delta /= 2;
				}

				timer += delta;
			}
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
				var direction = (laneEnds[lane].position - lanes[lane].position).normalized;

				var signedDistance = Vector3.Dot(
					note.transform.position - laneEnds[lane].position,
					direction
				);
				if (!(signedDistance > BadWindow)) break;

				queue.Dequeue();
				OnMissNote();
				CreateComboText(NoteJudgementResult.Miss);

				activeNotes[lane].Remove(note);

				Destroy(note.gameObject);
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
				OnHitNote(result);
				CreateComboText(result);
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

		private void LevelUp(float delta)
		{
			level++;
			var speed = currentSpeed + delta;
			currentSpeed = Math.Clamp(speed, 0, 3);
			audioSource.pitch = speed;
		}

		private void PlayGlow(int lane)
		{
			noteGlows[lane].SetActive(true);
			glowTweens[lane].Restart();
		}

		private void RestartGame()
		{
			SceneController.Instance.RequestChangeScene(new RhythmGameSceneParameter());
		}

		private void Quit()
		{
			Application.Quit();
		}

		private void PauseGame()
		{
			isPaused = true;
			audioSource.Pause();
			character.IsPaused = true;
			Time.timeScale = 0;
		}

		private void DisplayPauseUI()
		{
			if (!isGameStarted) return;
			if (isGameOver) return;
			uiController.DisplayPauseUI();
		}

		private void ResumeGame()
		{
			isPaused = false;
			audioSource.Play();
			character.IsPaused = false;
			Time.timeScale = 1;
		}

		public void OnHitNote(NoteJudgementResult result)
		{
			CurrentCombo++;
			AccumNotes++;
			uiController.PlayFacePump();
			switch (result)
			{
				case NoteJudgementResult.Bad:
					BadCount++;
					CurrentScore += 400;
					break;
				case NoteJudgementResult.Good:
					GoodCount++;
					CurrentScore += 600;
					break;
				case NoteJudgementResult.Great:
					GreatCount++;
					CurrentScore += 800;
					break;
				case NoteJudgementResult.Perfect:
					PerfectCount++;
					CurrentScore += 1000;
					RemainLife++;
					break;
			}

			MaxCombo = Math.Max(MaxCombo, CurrentCombo);
			uiController.UpdateScore(CurrentScore);
			if (AccumNotes >= 50)
			{
				AccumNotes = 0;
				LevelUp(0.1f);
			}

			if (CurrentScore >= 1000000)
			{
				isPaused = true;
				isGameOver = true;
				uiController.ShowResultAsync(new ResultUIParameter()
				{
					Score = CurrentScore,
					Perfect = PerfectCount,
					Great = GreatCount,
					Good = GoodCount,
					Bad = BadCount,
					Miss = MissCount,
				});
			}
		}

		public void OnMissNote()
		{
			MissCount++;
			CurrentCombo = 0;
			RemainLife -= 1;
			uiController.UpdateScore(CurrentScore);
		}

		public void CreateComboText(NoteJudgementResult result)
		{
			var t = Instantiate(comboTextPrefab, comboTextPivot.position, Quaternion.identity);
			t.PlayComboText(CurrentCombo, result);
		}

		public void RequestShowResultAndEndGame(int trackId, ChartType chartType, double averageError)
		{
			// uiController.ShowResultAsync(GameDataService.GetMusicData(trackId).ThumbnailSprite, PerfectCount, GreatCount, GoodCount, BadCount, MissCount,
			// MaxCombo, CurrentScore, CalculateRank(), (float)CurrentScore / currentMaxScore, averageError,);
		}

		private void UpdateLife(int currentLife)
		{
			uiController.UpdateLifeGauge(currentLife, MaxLife);
			if (remainLife == 0)
			{
				isPaused = true;
				isGameOver = true;
				uiController.ShowResultAsync(new ResultUIParameter()
				{
					Score = CurrentScore,
					Perfect = PerfectCount,
					Great = GreatCount,
					Good = GoodCount,
					Bad = BadCount,
					Miss = MissCount,
				});
			}
		}
	}
}
