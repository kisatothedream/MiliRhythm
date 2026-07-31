using System;
using System.Collections.Generic;
using MilliRhythm.Data.Domain;
using MilliRhythm.Input;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.TrackSelectorUI
{
	public class TrackSelectorListPanel : MonoBehaviour, IUIInputListener
	{
		[SerializeField] private ScrollRect scrollRect;
		[SerializeField] private TrackSelectorList trackSelectorList;
		private Action<TrackSelectorListModel> onTrackSelectionChanged;

		public void Set(List<TrackSelectorListModel> models, Action<TrackSelectorListModel> action)
		{
			trackSelectorList.Set(models);
			onTrackSelectionChanged = action;
			trackSelectorList.OnSelectionChanged += onTrackSelectionChanged;
			trackSelectorList.OnSelectionChanged += EnsureItemVisible;
			this.RegisterUIInputListener();
		}

		public void Finish()
		{
			trackSelectorList.OnSelectionChanged -= onTrackSelectionChanged;
			trackSelectorList.OnSelectionChanged -= EnsureItemVisible;
			this.UnregisterUIInputListener();
		}

		public void ApplyFilter(Member filter)
		{
			trackSelectorList.FilterByVocal(filter);
		}

		public void OnNavigate(Vector2 value)
		{
			trackSelectorList.Navigate(value.ToUINavigationType());
		}
		
		private void EnsureItemVisible(TrackSelectorListModel _)
		{
			RectTransform item = trackSelectorList.selectedTrack.GetComponent<RectTransform>();
			var viewport = scrollRect.viewport;
			var content = scrollRect.content;

			Canvas.ForceUpdateCanvases();

			var itemBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
				viewport,
				item);

			var viewportRect = viewport.rect;
			var offset = 0f;

			if (itemBounds.min.x < viewportRect.xMin)
			{
				offset = itemBounds.min.x - viewportRect.xMin;
			}
			else if (itemBounds.max.x > viewportRect.xMax)
			{
				offset = itemBounds.max.x - viewportRect.xMax;
			}

			if (Mathf.Approximately(offset, 0f))
				return;

			var position = content.anchoredPosition;
			position.x -= offset;
			content.anchoredPosition = position;
		}

		public void OnSubmit(bool value)
		{
		}

		public void OnCancel(bool value)
		{
		}
		
	}
}
