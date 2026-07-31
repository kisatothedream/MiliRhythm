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
		public TrackSelectorListView selectedTrack { get; private set; }
		private int filteredItemCount;

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
			filteredItemCount = 0;
			for (var i = 0; i < listItems.Count; i++)
			{
				var item = listItems[i];
				if (!item.IsFiltered)
				{
					item.SetNavigationId(i);
					filteredItemCount++;
				}
			}

			SelectTrack(0);
			Debug.Log($"filteredItemCount {filteredItemCount}");
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

		public void SelectTrack(int navigationId) => SelectTrack(listItems.FirstOrDefault(item => item.NavigationId == navigationId));
		public void SelectTrack(TrackSelectorListView track) => OnClickViewButtonAction(track);

		private void OnClickViewButtonAction(TrackSelectorListView track)
		{
			selectedTrack?.Deselect();

			if (track == null)
			{
				OnSelectionChanged?.Invoke(null);
				return;
			}

			selectedTrack = track;
			selectedTrack.Select();
			OnSelectionChanged?.Invoke(selectedTrack.Model);
		}

		public void Navigate(UINavigationType navigationType)
		{
			var currentNavigationId = selectedTrack.NavigationId;
			switch (navigationType)
			{
				case UINavigationType.Up:
					currentNavigationId = Mathf.Clamp(currentNavigationId - 1, 0, filteredItemCount - 1);
					break;
				case UINavigationType.Down:
					currentNavigationId = Mathf.Clamp(currentNavigationId + 1, 0, filteredItemCount - 1);
					break;
				case UINavigationType.Left:
					currentNavigationId = Mathf.Clamp(currentNavigationId - 3, 0, filteredItemCount - 1);
					break;
				case UINavigationType.Right:
					currentNavigationId = Mathf.Clamp(currentNavigationId + 3, 0, filteredItemCount - 1);
					break;
			}

			SelectTrack(currentNavigationId);
		}
	}
}
