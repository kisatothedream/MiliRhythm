#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.Repository;
using UnityEditor;
using UnityEngine;

namespace MilliRhythm.Data.Editor
{
	[InitializeOnLoad]
	public static class LocalizationEditorData
	{
		private static LocalizationDataRepository repository;

		public static bool Initialized { get; private set; }
		public static bool IsLoading { get; private set; }

		public static LanguageType CurrentLanguage { get; private set; } = LanguageType.Korean;

		public static IReadOnlyList<string> Keys { get; private set; } = Array.Empty<string>();

		public static event Action DataReloaded;

		static LocalizationEditorData()
		{
			EditorApplication.delayCall += Reload;
		}

		public static void SetLanguage(LanguageType language)
		{
			if (CurrentLanguage == language && Initialized)
				return;

			CurrentLanguage = language;
			Reload();
		}

		public static void Reload()
		{
			ReloadAsync().Forget();
		}

		private static async UniTaskVoid ReloadAsync()
		{
			if (IsLoading)
				return;

			if (EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			IsLoading = true;
			Initialized = false;

			try
			{
				var newRepository = new LocalizationDataRepository(CurrentLanguage);
				await newRepository.LoadAsync();

				repository = newRepository;
				Keys = repository.All.Keys.OrderBy(key => key).ToArray();

				Initialized = true;
				DataReloaded?.Invoke();
			}
			catch (Exception exception)
			{
				repository = null;
				Keys = Array.Empty<string>();

				Debug.LogException(exception);
			}
			finally
			{
				IsLoading = false;
			}
		}

		public static LocalizationData Get(string key)
		{
			EnsureInitialized();
			return repository.Get(key);
		}

		public static string GetText(string key)
		{
			EnsureInitialized();
			return repository.Get(key).Text;
		}

		public static bool TryGet(string key, out LocalizationData data)
		{
			if (!Initialized || repository == null)
			{
				data = null;
				return false;
			}

			return repository.TryGet(key, out data);
		}

		private static void EnsureInitialized()
		{
			if (!Initialized || repository == null)
			{
				throw new InvalidOperationException("Editor localization data is not initialized.");
			}
		}
	}
}

#endif
