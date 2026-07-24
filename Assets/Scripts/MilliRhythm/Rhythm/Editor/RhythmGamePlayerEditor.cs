using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace MilliRhythm.Rhythm.Editor
{
	[CustomEditor(typeof(RhythmGamePlayer))]
	public class RhythmGamePlayerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var t = (RhythmGamePlayer)target;

			if (GUILayout.Button("Play"))
			{
				t.StartGame();
			}
		}
		
	}
}
