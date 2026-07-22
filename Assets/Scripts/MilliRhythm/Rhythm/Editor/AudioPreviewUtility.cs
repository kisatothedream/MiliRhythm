using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MilliRhythm.Rhythm.Editor
{
	public static class AudioPreviewUtility
	{
		private static readonly Type AudioUtilType;
		private static readonly MethodInfo PlayPreviewClipMethod;
		private static readonly MethodInfo StopAllPreviewClipsMethod;

		static AudioPreviewUtility()
		{
			AudioUtilType = typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");

			PlayPreviewClipMethod =
				FindMethod("PlayPreviewClip") ??
				FindMethod("PlayClip");

			StopAllPreviewClipsMethod =
				FindMethod("StopAllPreviewClips") ??
				FindMethod("StopAllClips");
		}

		public static void Play(AudioClip audioClip, float startTime)
		{
			if (audioClip == null || PlayPreviewClipMethod == null)
			{
				return;
			}

			var startSample = Mathf.Clamp(
				Mathf.RoundToInt(startTime * audioClip.frequency),
				0,
				Mathf.Max(0, audioClip.samples - 1));

			var parameters = PlayPreviewClipMethod.GetParameters();

			if (parameters.Length == 3)
			{
				PlayPreviewClipMethod.Invoke(null, new object[]
				{
					audioClip,
					startSample,
					false,
				});

				return;
			}

			if (parameters.Length == 2)
			{
				PlayPreviewClipMethod.Invoke(null, new object[]
				{
					audioClip,
					startSample,
				});

				return;
			}

			if (parameters.Length == 1)
			{
				PlayPreviewClipMethod.Invoke(null, new object[]
				{
					audioClip,
				});
			}
		}

		public static void Stop()
		{
			StopAllPreviewClipsMethod?.Invoke(null, null);
		}

		private static MethodInfo FindMethod(string methodName)
		{
			return AudioUtilType?.GetMethod(
				methodName,
				BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}
	}
}
