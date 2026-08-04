using MilliRhythm.Data.Domain;
using MilliRhythm.UI.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MilliRhythm.UI.TrackSelectorUI
{
	public class TrackSelectorListModel : UIListItemModelBase<int>
	{
		public TrackSelectorListModel(int id) : base(id)
		{
		}

		public Sprite ThumbnailSprite;
		public AssetReferenceSprite JacketSpriteReference;
		public AudioClip PreviewAudioClip;
		public AssetReferenceT<AudioClip> AudioClipReference;
		public string TrackNameKey;
		public Member TrackVocal;
		public string Rank;
	}
}
