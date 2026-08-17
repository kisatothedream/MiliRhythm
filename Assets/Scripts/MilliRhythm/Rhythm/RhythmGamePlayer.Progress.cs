using System;
using MilliRhythm.Audio;
using MilliRhythm.Data.Domain;
using MilliRhythm.Data.GameDataService;
using MilliRhythm.Scene.Contracts;
using MilliRhythm.User;
using UnityEngine;

namespace MilliRhythm.Rhythm
{
	public partial class RhythmGamePlayer
	{
		[SerializeField] private ComboText comboTextPrefab;
		[SerializeField] private Transform comboTextPivot;

		private const int MaxLife = 300;

		private int RemainLife
		{
			get => remainLife;
			set
			{
				remainLife = Math.Clamp(value, 0, MaxLife);
				UpdateLifeGuage(remainLife);
			}
		}

		private int remainLife;

		private int MaxCombo;
		private int CurrentCombo;
		private int CurrentScore;
		private int currentMaxScore;
		private int PerfectCount;
		private int GreatCount;
		private int GoodCount;
		private int BadCount;
		private int MissCount;

		private void InitializeLife()
		{
			//if Modifier, Apply it
			RemainLife = 200;
		}

		private void OnHitNote(NoteJudgementResult result)
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

			currentMaxScore += 1000;

			MaxCombo = Math.Max(MaxCombo, CurrentCombo);
			uiController.UpdateScore(CurrentScore, (float)CurrentScore / currentMaxScore);
			SfxAudioPlayer.Instance.Play(SfxType.NoteReaction);
		}

		private void OnMissNote()
		{
			MissCount++;
			CurrentCombo = 0;
			RemainLife -= 15;
			currentMaxScore += 1000;
			SfxAudioPlayer.Instance.Play(SfxType.Bonk);
			uiController.UpdateScore(CurrentScore, (float)CurrentScore / currentMaxScore);
		}

		private void CreateComboText(NoteJudgementResult result)
		{
			var t = Instantiate(comboTextPrefab, comboTextPivot.position, Quaternion.identity);
			t.PlayComboText(CurrentCombo, result);
		}

		private void SendScore(int trackId, ChartType chartType)
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
				Rank = CalculateRank(),
			}));
			UserManager.RequestSave();
		}

		private void RequestShowResultAndEndGame(int trackId, ChartType chartType, double averageError, MusicSelectorSceneParameter parameter)
		{
			SendScore(trackId, chartType);
			uiController.ShowResultAsync(GameDataService.GetMusicData(trackId).ThumbnailSprite, PerfectCount, GreatCount, GoodCount, BadCount, MissCount,
				MaxCombo, CurrentScore, CalculateRank(), (float)CurrentScore / currentMaxScore, averageError, parameter);
		}

		private void UpdateLifeGuage(int currentLife)
		{
			uiController.UpdateLifeGauge(currentLife, MaxLife);
			if (currentLife <= 0)
			{
			}
		}

		private Rank CalculateRank()
		{
			var rate = (float)CurrentScore / currentMaxScore;
			var rank = rate switch
			{
				> 0.95f => Rank.M,
				> 0.9f => Rank.S,
				> 0.8f => Rank.A,
				> 0.7f => Rank.B,
				_ => Rank.C
			};
			Debug.Log(rank.ToString());
			return rank;
		}
	}
}
