using System;
using System.Collections.Generic;
using MilliRhythm.Input;
using UnityEngine;

namespace MilliRhythm.UI.TrackSelectorUI
{
	public class TrackSelectorListPanel : MonoBehaviour, IUIInputListener
	{
		[SerializeField] private TrackSelectorList trackSelectorList;
		private Action<TrackSelectorListModel> onTrackSelectionChanged;

		public void Set(List<TrackSelectorListModel> models, Action<TrackSelectorListModel> action)
		{
			trackSelectorList.Set(models);
			onTrackSelectionChanged = action;
			trackSelectorList.OnSelectionChanged += onTrackSelectionChanged;
			this.RegisterUIInputListener();
		}

		public void Finish()
		{
			trackSelectorList.OnSelectionChanged -= onTrackSelectionChanged;
			this.UnregisterUIInputListener();
		}

		public void OnNavigate(Vector2 value)
		{
			if (value.x > 0.5f)
			{
				trackSelectorList.Navigate(UINavigationType.Right);
			}
			else if (value.x < -0.5f)
			{
				trackSelectorList.Navigate(UINavigationType.Left);
			}
			else if (value.y > 0.5f)
			{
				trackSelectorList.Navigate(UINavigationType.Up);
			}
			else if (value.y < -0.5f)
			{
				trackSelectorList.Navigate(UINavigationType.Down);
			}
		}

		public void OnSubmit(bool value)
		{
		}

		public void OnCancel(bool value)
		{
		}
	}
}
