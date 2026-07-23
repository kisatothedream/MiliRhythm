using System.Linq;
using MilliRhythm.Data.Repository;
using MilliRhythm.Rhythm;
using UnityEditor;
using UnityEngine;

namespace MilliRhythm.Data.Editor
{
	[CustomEditor(typeof(RhythmChartDataScriptableObject))]
	public class RhythmChartDataEditor : UnityEditor.Editor
	{
		private const string RhythmChartDataFolderPath = "Assets/Data/Chart";

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var t = (RhythmChartDataScriptableObject)target;
			if (GUILayout.Button("ChartData 목록 갱신"))
			{
				RefreshChartDataList();
			}

			if (GUILayout.Button("음악 아이디로 정렬"))
			{
				t.Sort();
			}

			return;

			void RefreshChartDataList()
			{
				var rhythmChartData = AssetDatabase
					.FindAssets("t:RhythmChart", new[] { RhythmChartDataFolderPath })
					.Select(AssetDatabase.GUIDToAssetPath)
					.Select(AssetDatabase.LoadAssetAtPath<RhythmChart>)
					.Where(rhythmChart => rhythmChart != null)
					.OrderBy(rhythmChart => rhythmChart.MusicId)
					.ToList();

				Undo.RecordObject(t, "Refresh Music Data List");

				t.RegisterAllChartData(rhythmChartData);

				EditorUtility.SetDirty(t);
				AssetDatabase.SaveAssets();

				Debug.Log($"MusicData {rhythmChartData.Count}개를 등록했습니다.", t);
			}
		}
	}
}
