using System;
using System.Collections.Generic;
using System.Linq;
using MilliRhythm.Data.Domain;

namespace MilliRhythm.UI.TrackSelectorUI
{
	public class TrackSelectorList : UIListBase<TrackSelectorListModel, TrackSelectorListView, int>
	{
		public Action<TrackSelectorListModel> OnSelectionChanged;
		private TrackSelectorListModel selectedTrack;

		public override void Set(List<TrackSelectorListModel> models)
		{
			base.Set(models);
			ResetNavigationId();
		}

		public void ResetNavigationId()
		{
			for (var i = 0; i < listItems.Count; i++)
			{
				var item = listItems[i];
				if (!item.IsFiltered)
				{
					item.SetNavigationId(i);
				}
			}
		}

		public void ClearFilter()
		{
			foreach (var item in listItems)
			{
				item.SetFiltered(false);
			}

			ResetNavigationId();
		}

		public void FilterByVocal(Member filter)
		{
			ClearFilter();
			foreach (var filteredItem in listItems.Where(item => (item.Model.TrackVocal & filter) == 0).ToArray())
			{
				filteredItem.SetFiltered(true);
			}

			ResetNavigationId();
		}
	}
}
