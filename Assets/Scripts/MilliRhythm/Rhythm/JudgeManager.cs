using System;
using MilliRhythm.Data.Domain;
using MilliRhythm.Data.GameDataService;
using MilliRhythm.Scene.Contracts;
using MilliRhythm.UI.RhythmGameUI;
using MilliRhythm.User;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MilliRhythm.Rhythm
{
	public class JudgeManager : MonoBehaviour
	{
		[SerializeField] private RhythmGameUIController uiController;
		[SerializeField] private ComboText comboTextPrefab;
		[SerializeField] private Transform comboTextPivot;

		public const int MaxLife = 300;

		public int RemainLife
		{
			get => remainLife;
			set
			{
				remainLife = Math.Clamp(value, 0, MaxLife);
				UpdateLifeGuage(remainLife);
			}
		}

		private int remainLife;

		public int MaxCombo;
		public int CurrentCombo;
		public int CurrentScore;
		private int currentMaxScore;
		public int PerfectCount;
		public int GreatCount;
		public int GoodCount;
		public int BadCount;
		public int MissCount;

		public void Initialize()
		{
			//if Modifier, Apply it
			RemainLife = 200;
		}

		public void OnHitNote(NoteJudgementResult result)
		{
			CurrentCombo++;
			switch (result)
			{
				case NoteJudgementResult.Bad:
					BadCount++;
					CurrentScore += 400;
					break;
				case NoteJudgementResult.Good:
					GoodCount++;
					CurrentScore += 600;
					RemainLife++;
					break;
				case NoteJudgementResult.Great:
					GreatCount++;
					CurrentScore += 800;
					RemainLife++;
					break;
				case NoteJudgementResult.Perfect:
					PerfectCount++;
					CurrentScore += 1000;
					RemainLife++;
					break;
			}

			currentMaxScore += 1000;

			MaxCombo = Math.Max(MaxCombo, CurrentCombo);
			uiController.UpdateScore(CurrentScore, (float)CurrentScore / currentMaxScore);
		}

		public void OnMissNote()
		{
			MissCount++;
			CurrentCombo = 0;
			RemainLife -= 30;
			currentMaxScore += 1000;
			uiController.UpdateScore(CurrentScore, (float)CurrentScore / currentMaxScore);
		}

		public void CreateComboText(NoteJudgementResult result)
		{
			var t = Instantiate(comboTextPrefab, comboTextPivot.position, Quaternion.identity);
			t.PlayComboText(CurrentCombo, result);
		}

		public void SendScore(int trackId, ChartType chartType)
		{
			UserManager.SendRequest(Request.Create(new SetScoreEvent()
			{
				TrackId = trackId,
				ChartType = chartType,
				PerfectCount = PerfectCount,
				GreatCount = GreatCount,
				GoodCount = GoodCount,
				BadCount = BadCount,
				MissCount = MissCount,
				Combo = MaxCombo,
				Score = CurrentScore,
			}));
			UserManager.RequestSave();
		}

		public void RequestShowResultAndEndGame(int trackId, ChartType chartType, MusicSelectorSceneParameter parameter)
		{
			SendScore(trackId, chartType);
			uiController.ShowResultAsync(GameDataService.GetMusicData(trackId).ThumbnailSprite, PerfectCount, GreatCount, GoodCount, BadCount, MissCount, MaxCombo, CurrentScore, parameter);
		}

		private void UpdateLifeGuage(int currentLife)
		{
			uiController.UpdateLifeGauge(currentLife, MaxLife);
		}
	}
}
