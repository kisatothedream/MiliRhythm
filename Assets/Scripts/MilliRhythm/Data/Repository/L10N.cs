using System;

namespace MilliRhythm.Data.Repository
{
	public static class L10N
	{
		public static event Action OnLanguageChanged;

		public static void RefreshLanguage()
		{
			OnLanguageChanged?.Invoke();
		}
	}
}
