using System;
using UnityEngine.EventSystems;

namespace MilliRhythm.UI.Components
{
	public abstract class SelectableListViewBase<TModel, TKey> : UIListItemViewBase<TModel, TKey>, IPointerClickHandler
		where TModel : SelectableListModelBase<TKey>
		where TKey : IEquatable<TKey>
	{
		private Action<SelectableListViewBase<TModel, TKey>> onSelectAction;
		public bool IsSelected;

		public void RegisterOnSelectAction(Action<SelectableListViewBase<TModel, TKey>> action)
		{
			onSelectAction = action;
		}

		public void Select()
		{
			IsSelected = true;
			UpdateUI(IsSelected);
		}

		public void Deselect()
		{
			IsSelected = false;
			UpdateUI(IsSelected);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			onSelectAction?.Invoke(this);
		}
	}
}
