using MilliRhythm.Data.Domain;
using MilliRhythm.UI.Components;

namespace MilliRhythm.UI.TrackSelectorUI.Filter
{
	public class VocalFilterSelectableList : MultipleSelectableListBase<VocalFilterSelectableListModel, VocalFilterSelectableListView, int>
	{
		public void ApplyLastFilter(Member filter)
		{
			foreach (var item in listItems)
			{
				if (filter.HasFlag(item.Model.Member))
				{
					item.Select();
				}
				else
				{
					item.Deselect();
				}
			}
		}
	}
}
