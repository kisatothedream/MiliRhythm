using System;
using System.Collections.Generic;
using System.Linq;
using MilliRhythm.Data.Domain;

namespace MilliRhythm.UI.TrackSelectorUI
{
	public class TrackSelectorList : UIListBase<TrackSelectorListModel, TrackSelectorListView, int>
	{
		public Action<TrackSelectorListModel> OnSelectionChanged;
		private TrackSelectorListView selectedTrack;

		public override void Set(List<TrackSelectorListModel> models)
		{
			Clear();
			foreach (var model in models)
			{
				var item = Instantiate(listItemPrefab, content);
				listItems.Add(item);
				item.Set(model);
				item.SetButtonAction(OnClickViewButtonAction);
			}

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

			OnClickViewButtonAction(listItems.First(item => item.NavigationId == 0));
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

		private void OnClickViewButtonAction(TrackSelectorListView track)
		{
			selectedTrack?.Deselect();
			selectedTrack = track;
			selectedTrack.Select();
			OnSelectionChanged?.Invoke(selectedTrack.Model);
		}
	}
}
