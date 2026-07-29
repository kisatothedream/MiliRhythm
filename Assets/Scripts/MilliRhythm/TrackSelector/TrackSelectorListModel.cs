using MilliRhythm.Data.Common;
using MilliRhythm.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MilliRhythm.TrackSelector
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
		public string TrackName;
		public Member TrackVocal;
		public string Rank;
	}
}
