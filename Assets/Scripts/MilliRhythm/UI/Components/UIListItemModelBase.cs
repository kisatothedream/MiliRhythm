using System;

namespace MilliRhythm.UI.Components
{
	public class UIListItemModelBase<TId> where TId : IEquatable<TId>
	{
		public readonly TId Id;

		public UIListItemModelBase(TId id)
		{
			Id = id;
		}
	}
}
