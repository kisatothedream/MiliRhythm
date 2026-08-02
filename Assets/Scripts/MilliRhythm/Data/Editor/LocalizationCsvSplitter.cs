using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using MilliRhythm.Data.Repository;
using UnityEditor;
using UnityEngine;

namespace MilliRhythm.Data.Editor
{
	public static class LocalizationCsvSplitter
	{
		private const string SourcePath =
			"Assets/Data/Localization/Localization.csv";

		private const string OutputDirectory =
			"Assets/StreamingAssets/Localization";

		[MenuItem("Tools/Localization/Split Localization CSV")]
		public static void SplitLocalizationCsv()
		{
			try
			{
				var sourceData = LoadSourceData();

				Validate(sourceData);

				Directory.CreateDirectory(OutputDirectory);

				WriteLanguageFile(
					"Localization_ko.csv",
					sourceData,
					data => data.Text_KO);

				WriteLanguageFile(
					"Localization_en.csv",
					sourceData,
					data => data.Text_EN);

				WriteLanguageFile(
					"Localization_jp.csv",
					sourceData,
					data => data.Text_JP);

				AssetDatabase.Refresh();

				Debug.Log(
					$"Localization CSV split completed. " +
					$"Entry count: {sourceData.Count}");
			}
			catch (Exception e)
			{
				Debug.LogException(
					new Exception(
						$"Failed to split localization CSV. Source: {SourcePath}",
						e));
			}
		}

		private static List<LocalizationSourceData> LoadSourceData()
		{
			if (!File.Exists(SourcePath))
			{
				throw new FileNotFoundException(
					"Localization source CSV was not found.",
					SourcePath);
			}

			using var reader = new StreamReader(SourcePath);
			using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

			return csv.GetRecords<LocalizationSourceData>().ToList();
		}

		private static void Validate(
			IReadOnlyCollection<LocalizationSourceData> sourceData)
		{
			if (sourceData.Count == 0)
			{
				throw new InvalidDataException(
					"Localization source CSV contains no data.");
			}

			var emptyId = sourceData.FirstOrDefault(data => string.IsNullOrWhiteSpace(data.Id));

			if (emptyId != null)
			{
				throw new InvalidDataException(
					"Localization source CSV contains an empty ID.");
			}

			var duplicatedIds = sourceData
				.GroupBy(data => data.Id)
				.Where(group => group.Count() > 1)
				.Select(group => group.Key)
				.ToArray();

			if (duplicatedIds.Length > 0)
			{
				throw new InvalidDataException(
					$"Duplicate localization IDs: " +
					$"{string.Join(", ", duplicatedIds)}");
			}

			foreach (var data in sourceData)
			{
				ValidateText(data.Id, "Korean", data.Text_KO);
				ValidateText(data.Id, "English", data.Text_EN);
				ValidateText(data.Id, "Japanese", data.Text_JP);
			}
		}

		private static void ValidateText(
			string id,
			string language,
			string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				return;
			}

			throw new InvalidDataException(
				$"Localization text is empty. " +
				$"Id: {id}, Language: {language}");
		}

		private static void WriteLanguageFile(
			string fileName,
			IEnumerable<LocalizationSourceData> sourceData,
			Func<LocalizationSourceData, string> textSelector)
		{
			var outputPath = Path.Combine(OutputDirectory, fileName);

			var outputData = sourceData
				.Select(data => new LocalizationData()
				{
					Id = data.Id,
					Text = textSelector(data)
				})
				.ToList();

			using var writer = new StreamWriter(
				outputPath,
				false,
				new System.Text.UTF8Encoding(false));

			using var csv = new CsvWriter(
				writer,
				CultureInfo.InvariantCulture);

			csv.WriteRecords(outputData);
		}

		private sealed class LocalizationSourceData
		{
			public string Id { get; set; }
			public string Text_KO { get; set; }
			public string Text_EN { get; set; }
			public string Text_JP { get; set; }
		}
	}
}
