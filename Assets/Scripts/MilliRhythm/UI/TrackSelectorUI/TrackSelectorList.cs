using System;
using System.Collections.Generic;
using System.Linq;
using MilliRhythm.Data.Domain;
using MilliRhythm.UI.Components;
using UnityEngine;

namespace MilliRhythm.UI.TrackSelectorUI
{
	public class TrackSelectorList : UIListBase<TrackSelectorListModel, TrackSelectorListView, int>
	{
		public Action<TrackSelectorListModel> OnSelectionChanged;
		public TrackSelectorListView SelectedTrack { get; private set; }
		private int visibleItemCount;

		public override void Set(List<TrackSelectorListModel> models)
		{
			Clear();
			foreach (var model in models)
			{
				var item = Instantiate(listItemPrefab, content);
				listItems.Add(item);
				item.Set(model);
				item.SetButtonAction(OnClickViewButtonAction);
				item.SetVisible(true);
			}

			ResetNavigationId();
		}

		public void ResetNavigationId()
		{
			var id = 0;
			visibleItemCount = 0;
			for (var i = 0; i < listItems.Count; i++)
			{
				var item = listItems[i];
				if (item.IsVisible)
				{
					item.SetNavigationId(id++);
					visibleItemCount++;
				}
			}

			SelectTrack(0);
		}

		public void ClearFilter()
		{
			for (var i = 0; i < listItems.Count; i++)
			{
				var item = listItems[i];
				item.SetVisible(true);
			}

			ResetNavigationId();
		}

		public void FilterByVocal(Member filter)
		{
			ClearFilter();
			if (filter == 0)
			{
				return;
			}

			foreach (var item in listItems)
			{
				if (!filter.HasFlag(item.Model.TrackVocal))
				{
					item.SetVisible(false);
				}
			}

			ResetNavigationId();
		}

		public void SelectTrack(int navigationId) => SelectTrack(listItems.FirstOrDefault(item => item.IsVisible && item.NavigationId == navigationId));
		public void SelectTrack(TrackSelectorListView track) => OnClickViewButtonAction(track);

		private void OnClickViewButtonAction(TrackSelectorListView track)
		{
			SelectedTrack?.Deselect();

			if (track == null)
			{
				OnSelectionChanged?.Invoke(null);
				return;
			}

			SelectedTrack = track;
			SelectedTrack.Select();
			OnSelectionChanged?.Invoke(SelectedTrack.Model);
		}

		public void Navigate(UINavigationType navigationType)
		{
			var currentNavigationId = SelectedTrack.NavigationId;
			switch (navigationType)
			{
				case UINavigationType.Up:
					currentNavigationId = Mathf.Clamp(currentNavigationId - 1, 0, visibleItemCount - 1);
					break;
				case UINavigationType.Down:
					currentNavigationId = Mathf.Clamp(currentNavigationId + 1, 0, visibleItemCount - 1);
					break;
				case UINavigationType.Left:
					currentNavigationId = Mathf.Clamp(currentNavigationId - 3, 0, visibleItemCount - 1);
					break;
				case UINavigationType.Right:
					currentNavigationId = Mathf.Clamp(currentNavigationId + 3, 0, visibleItemCount - 1);
					break;
			}

			SelectTrack(currentNavigationId);
		}
	}
}
