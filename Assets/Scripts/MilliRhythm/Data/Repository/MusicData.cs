using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.Domain;
using MilliRhythm.Data.GameDataService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MilliRhythm.Data.Repository
{
	[CreateAssetMenu(fileName = "MusicData", menuName = "MilliRhythm/Data/MusicData")]
	public class MusicData : ScriptableObject
	{
		[field: SerializeField] public int Id { get; private set; }

		//Non-localized Text
		[field: SerializeField] public string Name { get; private set; }
		[field: SerializeField] public string NameKey { get; private set; }
		[field: SerializeField] public Member Vocals { get; private set; }

		//Resource
		[field: SerializeField] public Sprite ThumbnailSprite { get; private set; }
		[field: SerializeField] public AssetReferenceSprite JacketSprite { get; private set; }
		[field: SerializeField] public AudioClip PreviewAudioClip { get; private set; }
		[field: SerializeField] public AssetReferenceT<AudioClip> AudioClipReference { get; private set; }
	}
}
