using System;

namespace MilliRhythm.UI.Components
{
	public class SelectableListModelBase<TKey> : UIListItemModelBase<TKey> where TKey : IEquatable<TKey>
	{
		public SelectableListModelBase(TKey id) : base(id)
		{
		}
	}
}
