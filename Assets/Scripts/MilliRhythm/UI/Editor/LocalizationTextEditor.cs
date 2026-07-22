using System;
using MilliRhythm.Data.Editor;
using MilliRhythm.Data.Repository;
using MilliRhythm.UI.Components;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace MilliRhythm.UI.Editor
{
	[CustomEditor(typeof(LocalizedText))]
	public sealed class LocalizedTextEditor : UnityEditor.Editor
	{
		private SerializedProperty localizationKeyProperty;
		private TextMeshProUGUI textComponent;

		private void OnEnable()
		{
			localizationKeyProperty = serializedObject.FindProperty("localizationKey");

			var localizedText = (LocalizedText)target;
			textComponent = localizedText.GetComponent<TextMeshProUGUI>();

			LocalizationEditorData.DataReloaded += OnLocalizationDataReloaded;
		}

		private void OnDisable()
		{
			LocalizationEditorData.DataReloaded -= OnLocalizationDataReloaded;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawLanguageSelector();
			EditorGUILayout.Space();

			DrawLocalizationSelector();
			EditorGUILayout.Space();

			DrawPreview();
			EditorGUILayout.Space();

			DrawUtilityButtons();

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawLanguageSelector()
		{
			var currentLanguage = LocalizationEditorData.CurrentLanguage;

			var selectedLanguage = (LanguageType)EditorGUILayout.EnumPopup("Preview Language", currentLanguage);

			if (selectedLanguage != currentLanguage)
				LocalizationEditorData.SetLanguage(selectedLanguage);
		}

		private void DrawLocalizationSelector()
		{
			if (LocalizationEditorData.IsLoading)
			{
				EditorGUILayout.HelpBox("Localization 데이터를 불러오는 중입니다.", MessageType.Info);

				EditorGUILayout.PropertyField(localizationKeyProperty);
				return;
			}

			if (!LocalizationEditorData.Initialized)
			{
				EditorGUILayout.HelpBox("Localization 데이터가 초기화되지 않았습니다.", MessageType.Warning);

				EditorGUILayout.PropertyField(localizationKeyProperty);
				return;
			}

			var keys = LocalizationEditorData.Keys;

			if (keys.Count == 0)
			{
				EditorGUILayout.HelpBox("사용 가능한 Localization Key가 없습니다.", MessageType.Warning);

				EditorGUILayout.PropertyField(localizationKeyProperty);
				return;
			}

			var currentIndex = FindCurrentIndex(keys, localizationKeyProperty.stringValue);

			var displayNames = new string[keys.Count + 1];
			displayNames[0] = "None";

			for (var i = 0; i < keys.Count; i++)
				displayNames[i + 1] = keys[i];

			var selectedIndex = EditorGUILayout.Popup("Localization Key", currentIndex + 1, displayNames);

			if (selectedIndex == currentIndex + 1)
				return;

			Undo.RecordObject(target, "Change Localization Key");

			localizationKeyProperty.stringValue = selectedIndex == 0 ? string.Empty : keys[selectedIndex - 1];

			serializedObject.ApplyModifiedProperties();

			ApplyEditorPreview();
			EditorUtility.SetDirty(target);
		}

		private void DrawPreview()
		{
			var key = localizationKeyProperty.stringValue;

			if (string.IsNullOrWhiteSpace(key))
			{
				EditorGUILayout.HelpBox("Localization Key를 선택해 주세요.", MessageType.None);

				return;
			}

			if (!LocalizationEditorData.TryGet(key, out var data))
			{
				EditorGUILayout.HelpBox($"Localization Key를 찾을 수 없습니다: {key}", MessageType.Error);

				return;
			}

			EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

			using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
			{
				EditorGUILayout.SelectableLabel(data.Text, EditorStyles.wordWrappedLabel, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2f));
			}
		}

		private void DrawUtilityButtons()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				using (new EditorGUI.DisabledScope(LocalizationEditorData.IsLoading))
				{
					if (GUILayout.Button("Reload Localization"))
						LocalizationEditorData.Reload();
				}

				if (GUILayout.Button("Apply Preview"))
					ApplyEditorPreview();
			}
		}

		private void ApplyEditorPreview()
		{
			if (textComponent == null)
				return;

			var key = localizationKeyProperty.stringValue;

			if (string.IsNullOrWhiteSpace(key))
				return;

			if (!LocalizationEditorData.TryGet(key, out var data))
				return;

			Undo.RecordObject(textComponent, "Apply Localized Text Preview");

			textComponent.text = data.Text;

			EditorUtility.SetDirty(textComponent);
			PrefabUtility.RecordPrefabInstancePropertyModifications(textComponent);
		}

		private void OnLocalizationDataReloaded()
		{
			ApplyEditorPreview();
			Repaint();
		}

		private static int FindCurrentIndex(System.Collections.Generic.IReadOnlyList<string> keys, string currentKey)
		{
			for (var i = 0; i < keys.Count; i++)
			{
				if (string.Equals(keys[i], currentKey, StringComparison.Ordinal))
				{
					return i;
				}
			}

			return -1;
		}
	}
}
