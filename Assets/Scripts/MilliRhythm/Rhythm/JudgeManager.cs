using System;
using MilliRhythm.Data.Domain;

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
	}
}
