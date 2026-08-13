using System;
using MilliRhythm.Data.Domain;
using MilliRhythm.UI.RhythmGameUI;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	public class JudgeManager : MonoBehaviour
	{
		[SerializeField] private RhythmGameUIController uiController;
		[SerializeField] private ComboText comboTextPrefab;
		[SerializeField] private Transform comboTextPivot;

		public const int MaxLife = 5;

		public int RemainLife
		{
			get => remainLife;
			set
			{
				remainLife = Math.Clamp(value, 0, MaxLife);
				UpdateLifeGauge(remainLife);
			}
		}

		private int remainLife;

		public int MaxCombo;
		public int CurrentCombo;
		public int CurrentScore;
		public int PerfectCount;
		public int GreatCount;
		public int GoodCount;
		public int BadCount;
		public int MissCount;

		public void Initialize()
		{
			RemainLife = 5;
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
					RemainLife += 15;
					break;
				case NoteJudgementResult.Great:
					GreatCount++;
					CurrentScore += 800;
					RemainLife += 15;
					break;
				case NoteJudgementResult.Perfect:
					PerfectCount++;
					CurrentScore += 1000;
					RemainLife += 15;
					break;
			}

			MaxCombo = Math.Max(MaxCombo, CurrentCombo);
			uiController.UpdateScore(CurrentScore, 0);
		}

		public void OnMissNote()
		{
			MissCount++;
			CurrentCombo = 0;
			RemainLife -= 1;
			uiController.UpdateScore(CurrentScore, 0);
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

		private void UpdateLifeGauge(int currentLife)
		{
			uiController.UpdateLifeGauge(currentLife, MaxLife);
		}
	}
}
