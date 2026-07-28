using System;
using MilliRhythm.UI;

namespace MilliRhythm.TrackSelector
{
	public class TrackSelectorList : UIListBase<TrackSelectorListModel, TrackSelectorListView, int>
	{
		private Action<TrackSelectorListModel> onSelectionChanged;
		private TrackSelectorListModel selectedTrack;
	}
}
