using MilliRhythm.UI.Components;
using UnityEditor;
using UnityEditor.UI;

namespace MilliRhythm.UI.Editor
{
	[CustomEditor(typeof(NavigatableSlider))]
	public class NavigatableSliderEditor : SliderEditor
	{
		private SerializedProperty focusIndicatorProperty;

		protected override void OnEnable()
		{
			base.OnEnable();
			focusIndicatorProperty = serializedObject.FindProperty("focusIndicator");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			serializedObject.Update();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Navigation", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(focusIndicatorProperty);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
