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

		public Sprite MiniJacketSprite;
		public AssetReferenceSprite JacketSpriteReference;
		public string trackName;
		public Member trackVocal;
		public string Rank;
	}
}
