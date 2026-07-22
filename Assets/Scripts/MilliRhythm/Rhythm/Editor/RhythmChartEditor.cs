using UnityEditor;
using UnityEngine;

namespace MilliRhythm.Rhythm.Editor
{
	[CustomEditor(typeof(RhythmChart))]
	public class RhythmChartInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var rhythmChart = (RhythmChart)target;

			EditorGUILayout.Space();

			if (GUILayout.Button(
				    "Open Rhythm Chart Editor",
				    GUILayout.Height(32f)))
			{
				RhythmChartEditorWindow.Open(rhythmChart);
			}
		}
	}
}
