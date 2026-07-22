using UnityEditor;
using UnityEngine;

namespace MilliRhythm.Rhythm.Editor
{
	[InitializeOnLoad]
	internal static class RhythmPreviewReloadCleaner
	{
		static RhythmPreviewReloadCleaner()
		{
			AssemblyReloadEvents.beforeAssemblyReload += Cleanup;

			EditorApplication.quitting += Cleanup;

			EditorApplication.delayCall += Cleanup;
		}

		private static void Cleanup()
		{
			var players = Resources.FindObjectsOfTypeAll<RhythmGamePlayer>();

			for (var i = 0; i < players.Length; i++)
			{
				var player = players[i];

				if (player == null || EditorUtility.IsPersistent(player) || !player.gameObject.scene.IsValid())
				{
					continue;
				}

				player.DestroyEditorPreview();
			}
		}
	}
}
