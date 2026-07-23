using System.Linq;
using MilliRhythm.Data.Repository;
using UnityEditor;
using UnityEngine;

namespace MilliRhythm.Data.Editor
{
	[CustomEditor(typeof(MusicDataScriptableObject))]
	public class MusicDataEditor : UnityEditor.Editor
	{
		private const string MusicDataFolderPath = "Assets/Data/Music";

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var t = (MusicDataScriptableObject)target;
			if (GUILayout.Button("MusicData 목록 갱신"))
			{
				RefreshMusicDataList();
			}

			if (GUILayout.Button("아이디로 정렬"))
			{
				t.Sort();
			}

			return;

			void RefreshMusicDataList()
			{
				var musicDataList = AssetDatabase
					.FindAssets("t:MusicData", new[] { MusicDataFolderPath })
					.Select(AssetDatabase.GUIDToAssetPath)
					.Select(AssetDatabase.LoadAssetAtPath<MusicData>)
					.Where(musicData => musicData != null)
					.OrderBy(musicData => musicData.name)
					.ToList();

				Undo.RecordObject(t, "Refresh Music Data List");
				t.RegisterAllMusicData(musicDataList);

				EditorUtility.SetDirty(t);
				AssetDatabase.SaveAssets();

				Debug.Log($"MusicData {musicDataList.Count}개를 등록했습니다.", t);
			}
		}
	}
}
