#if UNITY_EDITOR
using MilliRhythm.Data.Repository;
using UnityEditor;
using UnityEngine;

namespace MilliRhythm.Data.Editor
{
	public sealed class LocalizationDataWindow : EditorWindow
	{
		private Vector2 scrollPosition;
		private string searchText = string.Empty;

		[MenuItem("Tools/Localization/Data Viewer")]
		private static void Open()
		{
			GetWindow<LocalizationDataWindow>("Localization Data");
		}

		private void OnEnable()
		{
			LocalizationEditorData.DataReloaded += Repaint;
		}

		private void OnDisable()
		{
			LocalizationEditorData.DataReloaded -= Repaint;
		}

		private void OnGUI()
		{
			DrawLanguageSelector();

			if (LocalizationEditorData.IsLoading)
			{
				EditorGUILayout.HelpBox("Localization 데이터를 불러오는 중입니다.", MessageType.Info);

				return;
			}

			if (!LocalizationEditorData.Initialized)
			{
				EditorGUILayout.HelpBox("Localization 데이터가 로드되지 않았습니다.", MessageType.Warning);

				return;
			}

			DrawToolbar();
			DrawLocalizationList();
		}

		private void DrawToolbar()
		{
			using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				searchText = GUILayout.TextField(searchText, EditorStyles.toolbarSearchField);

				if (GUILayout.Button("Reload", EditorStyles.toolbarButton))
				{
					LocalizationEditorData.Reload();
				}
			}
		}

		private void DrawLocalizationList()
		{
			var keys = LocalizationEditorData.Keys;

			EditorGUILayout.LabelField($"Cached Entries: {keys.Count}", EditorStyles.boldLabel);

			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			foreach (var key in keys)
			{
				if (!IsSearchMatch(key))
					continue;

				if (!LocalizationEditorData.TryGet(key, out var data))
					continue;

				DrawLocalizationEntry(data);
			}

			EditorGUILayout.EndScrollView();
		}

		private bool IsSearchMatch(string key)
		{
			if (string.IsNullOrWhiteSpace(searchText))
				return true;

			return key.Contains(searchText, System.StringComparison.OrdinalIgnoreCase);
		}

		private static void DrawLocalizationEntry(LocalizationData data)
		{
			using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
			{
				EditorGUILayout.SelectableLabel(data.Id, EditorStyles.boldLabel, GUILayout.Height(EditorGUIUtility.singleLineHeight));
				EditorGUILayout.SelectableLabel(data.Text, EditorStyles.wordWrappedLabel, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight));
			}
		}

		private void DrawLanguageSelector()
		{
			var selectedLanguage = (LanguageType)EditorGUILayout.EnumPopup("Language", LocalizationEditorData.CurrentLanguage);

			if (selectedLanguage != LocalizationEditorData.CurrentLanguage)
			{
				LocalizationEditorData.SetLanguage(selectedLanguage);
			}

			using (new EditorGUI.DisabledScope(LocalizationEditorData.IsLoading))
			{
				if (GUILayout.Button("Reload"))
				{
					LocalizationEditorData.Reload();
				}
			}
		}
	}
}
#endif
