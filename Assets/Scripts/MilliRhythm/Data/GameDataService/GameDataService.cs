using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MilliRhythm.Config;
using MilliRhythm.Data.Domain;
using MilliRhythm.Data.Repository;
using UnityEngine;

/*
 * AI 도구의 도움을 받아 구현한 게임 정적 데이터 관리 시스템입니다.
 * CSV 데이터를 종류별 Repository로 로드하고 GameDataService에 등록하여, 게임 전역에서 정적 데이터에 접근할 수 있도록 설계했습니다.
 * 초기화는 게임 구동 시 Bootstrapper가 명시적으로 수행합니다.
 * Repository 기반 조회 구조를 사용하되, 사용 편의성을 위해 데이터 종류별 숏컷 조회 API를 partial class로 분리해 제공합니다.
 */
namespace MilliRhythm.Data.GameDataService
{
	public static partial class GameDataService
	{
		public static bool Initialized { get; internal set; }

		private static readonly Dictionary<Type, IGameDataRepository> repositories = new();

		internal static async UniTask Initialize()
		{
			await LoadGameDataAsync();
			ConfigManager.Instance.OnLanguageChangedAction += ChangeLanguage;
		}

		private static async UniTask LoadGameDataAsync()
		{
			Initialized = false;
			repositories.Clear();

			try
			{
				//TODO: 현재 언어 기반으로 변경
				var localizationRepository = new LocalizationDataRepository(ConfigManager.Instance.Config.Language);

				await localizationRepository.LoadAsync();
				Register(localizationRepository);

				await CreateAndRegisterAsync<MusicDataRepository>();
				await CreateAndRegisterAsync<RhythmChartRepository>();
				await CreateAndRegisterAsync<MemberDataRepository>();
				Initialized = true;

				Debug.Log("Game Static Data Loaded");
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		private static async UniTask CreateAndRegisterAsync<TRepository>() where TRepository : IGameDataRepository, new()
		{
			var repository = new TRepository();

			await repository.LoadAsync();

			Register(repository);
		}

		private static void Register<TRepository>(TRepository repository) where TRepository : IGameDataRepository
		{
			if (!repositories.TryAdd(typeof(TRepository), repository))
			{
				throw new InvalidOperationException($"Repository already registered. " + $"Type: {typeof(TRepository).Name}");
			}
		}

		private static void Replace(LocalizationDataRepository repository)
		{
			repositories[typeof(LocalizationDataRepository)] = repository;
		}

		public static TRepository GetData<TRepository>() where TRepository : IGameDataRepository
		{
			if (!Initialized)
			{
				throw new InvalidOperationException("GameDataService is not initialized yet.");
			}

			if (!repositories.TryGetValue(typeof(TRepository), out var repository))
			{
				throw new KeyNotFoundException($"Repository not found. Type: {typeof(TRepository).Name}");
			}

			return (TRepository)repository;
		}

		public static void ReleaseAllRepositories()
		{
			foreach (var repository in repositories.Values)
			{
				repository.Release();
			}
		}

		private static void ChangeLanguage(LanguageType type)
		{
			RequestReloadLanguage(type).Forget();
		}

		private static async UniTask RequestReloadLanguage(LanguageType type)
		{
			try
			{
				var last = repositories[typeof(LocalizationDataRepository)];
				var localizationRepository = new LocalizationDataRepository(type);
				await localizationRepository.LoadAsync();
				Replace(localizationRepository);
				last.Release();
			}
			catch (Exception e)
			{
				throw new Exception($"Failed to reload language. Type: {type}. Reason: {e.Message}", e);
			}

			L10N.RefreshLanguage();
		}
	}
}
