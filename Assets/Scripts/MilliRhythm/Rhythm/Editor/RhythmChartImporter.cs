using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MilliRhythm.Rhythm.Editor
{
	[Serializable]
	public class RhythmChartImportData
	{
		public double bpm;
		public double offsetSeconds;
		public List<RhythmNoteImportData> notes = new();
	}

	[Serializable]
	public class RhythmNoteImportData
	{
		public int head;
		public int lane;
		public int lengthTick;
	}

	public static class RhythmChartJsonImporter
	{
		[MenuItem("Assets/Rhythm Game/Import Chart From JSON", true)]
		private static bool ValidateImport()
		{
			return Selection.activeObject is RhythmChart;
		}

		[MenuItem("Assets/Rhythm Game/Import Chart From JSON")]
		private static void Import()
		{
			if (Selection.activeObject is not RhythmChart chart)
				return;

			var path = EditorUtility.OpenFilePanel(
				"Import Rhythm Chart",
				"",
				"json");

			if (string.IsNullOrEmpty(path))
				return;

			try
			{
				var json = File.ReadAllText(path);
				var data = JsonUtility.FromJson<RhythmChartImportData>(json);

				if (data == null)
				{
					Debug.LogError("Failed to deserialize rhythm chart JSON.");
					return;
				}

				Validate(data);

				Undo.RecordObject(chart, "Import Rhythm Chart");

				chart.SetBpm(data.bpm);
				chart.SetOffsetSeconds(data.offsetSeconds);

				chart.Notes.Clear();

				foreach (var note in data.notes)
				{
					chart.Notes.Add(new RhythmNote
					{
						Head = note.head,
						Lane = note.lane,
						LengthTick = note.lengthTick
					});
				}

				// 혹시 JSON의 노트 순서가 뒤섞여 있어도 정렬
				chart.Notes.Sort((a, b) => a.Head.CompareTo(b.Head));

				EditorUtility.SetDirty(chart);
				AssetDatabase.SaveAssets();

				Debug.Log(
					$"Imported Rhythm Chart: {chart.name}, " +
					$"BPM={data.bpm}, Notes={data.notes.Count}");
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		private static void Validate(RhythmChartImportData data)
		{
			if (data.bpm <= 0)
				throw new InvalidDataException($"Invalid BPM: {data.bpm}");

			if (data.notes == null)
				throw new InvalidDataException("Notes is null.");

			for (var i = 0; i < data.notes.Count; i++)
			{
				var note = data.notes[i];

				if (note.head < 0)
					throw new InvalidDataException(
						$"Note[{i}] has invalid Head: {note.head}");

				if (note.lane < 0 || note.lane >= 4)
					throw new InvalidDataException(
						$"Note[{i}] has invalid Lane: {note.lane}");

				if (note.lengthTick < 0)
					throw new InvalidDataException(
						$"Note[{i}] has invalid LengthTick: {note.lengthTick}");
			}
		}
	}
}
