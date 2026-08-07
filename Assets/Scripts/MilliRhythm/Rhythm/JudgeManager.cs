using System;
using MilliRhythm.Data.Domain;
using MilliRhythm.User;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MilliRhythm.Rhythm
{
	public class JudgeManager
	{
		public int MaxLife = 10;
		public int RemainLife;

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
			//if Modifier, Apply it
			RemainLife = MaxLife;
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
					break;
				case NoteJudgementResult.Great:
					GreatCount++;
					CurrentScore += 800;
					break;
				case NoteJudgementResult.Perfect:
					PerfectCount++;
					CurrentScore += 1000;
					break;
			}

			MaxCombo = Math.Max(MaxCombo, CurrentCombo);
		}

		public void OnMissNote()
		{
			MissCount++;
			CurrentCombo = 0;
		}

		public void CreateComboText(Vector3 position, ComboText text, NoteJudgementResult result)
		{
			var t = Object.Instantiate(text, position, Quaternion.identity);
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
	}
}
