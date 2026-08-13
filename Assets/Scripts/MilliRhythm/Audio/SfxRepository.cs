using System;
using System.Collections.Generic;
using UnityEngine;

namespace MilliRhythm.Audio
{
	[CreateAssetMenu(menuName = "MilliRhythm/Audio/Sfx Repository")]
	public class SfxRepository : ScriptableObject
	{
		[SerializeField] private List<SfxData> sfxLists;
		public IReadOnlyList<SfxData> SfxLists => sfxLists;
	}

	[Serializable]
	public class SfxData
	{
		public SfxType SfxType;
		public AudioClip Clip;
	}

	public enum SfxType
	{
		Confirm,
		Cancel,
		Navigate,
		NoteReaction,
		GameStart,
		GameOver,
	}
}
