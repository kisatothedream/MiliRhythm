using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using UnityEngine;
using UnityEngine.Networking;

namespace MilliRhythm.Data.GameDataService
{
	public static class CsvDataLoader
	{
		public static async Awaitable<List<TData>> LoadAsync<TData>(string fileName, CsvConfiguration configuration = null)
		{
			var path = Path.Combine(Application.streamingAssetsPath, fileName);
			var csvText = await ReadStreamingAssetsTextAsync(path);

			configuration ??= CreateDefaultConfiguration();

			using var reader = new StringReader(csvText);
			using var csv = new CsvReader(reader, configuration);

			return csv.GetRecords<TData>().ToList();
		}

		private static CsvConfiguration CreateDefaultConfiguration()
		{
			return new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = true,
				IgnoreBlankLines = true,
				TrimOptions = TrimOptions.Trim,
			};
		}

		private static async Awaitable<string> ReadStreamingAssetsTextAsync(string path)
		{
			using var request = UnityWebRequest.Get(path);

			await request.SendWebRequest();

			if (request.result != UnityWebRequest.Result.Success)
			{
				throw new IOException(
					$"Failed to load StreamingAssets file: {path}\n{request.error}");
			}

			return request.downloadHandler.text;
		}
	}
}
