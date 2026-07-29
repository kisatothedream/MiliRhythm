using System;
using MilliRhythm.UI;

namespace MilliRhythm.TrackSelector
{
	public class TrackSelectorList : UIListBase<TrackSelectorListModel, TrackSelectorListView, int>
	{
		public Action<TrackSelectorListModel> OnSelectionChanged;
		private TrackSelectorListModel selectedTrack;
	}
}
