using MilliRhythm.UI.Components;

namespace MilliRhythm.UI.TrackSelectorUI.Filter
{
	public class VocalFilterSelectableListView : SelectableListViewBase<VocalFilterSelectableListModel, int>
	{
	}

	public class VocalFilterSelectableListModel : SelectableListModelBase<int>
	{
		public VocalFilterSelectableListModel(int id) : base(id)
		{
		}
	}
}
