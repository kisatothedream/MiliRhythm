using System;
using System.Collections.Generic;
using System.Linq;
using MilliRhythm.Data.Domain;
using MilliRhythm.UI.Components;
using UnityEngine;

namespace MilliRhythm.UI.TrackSelectorUI.Filter
{
	public class VocalFilterPopup : PopupUIBase<VocalFilterPopupParameter, VocalFilterPopupResponse, VocalFilterPopupPayload>
	{
		[SerializeField] private VocalFilterSelectableList list;

		[SerializeField] private NavigatableButton confirmButtonNavigator;
		private INavigatable focusedNavigatableItem;
		private int focusedViewItemIndex;
		private int filterSize => Enum.GetValues(typeof(Member)).Length;

		protected override void Set()
		{
			focusedViewItemIndex = 0;
			response = new VocalFilterPopupResponse();
			response.Payload = new VocalFilterPopupPayload();
			var memberTypes = Enum.GetValues(typeof(Member));
			var models = new List<VocalFilterSelectableListModel>();

			for (int i = 0; i < memberTypes.Length; i++)
			{
				var member = (Member)memberTypes.GetValue(i);
				var model = new VocalFilterSelectableListModel(i, member);
				models.Add(model);
			}

			list.Set(models);
			list.ApplyLastFilter(parameter.LastFilter);

			FocusTo(focusedViewItemIndex);
		}

		protected override void Confirm()
		{
			var filter = (Member)0;
			foreach (var item in list.GetSelectedItems())
			{
				filter |= item.Model.Member;
			}

			response.Payload.FilterMember = filter;
			base.Confirm();
		}

		public override void OnNavigate(Vector2 value)
		{
			var direction = value.ToUINavigationType();
			var delta = 0;
			switch (direction)
			{
				case UINavigationType.Up:
					delta = -2;
					break;
				case UINavigationType.Down:
					delta = 2;
					break;
				case UINavigationType.Left:
					delta = -1;
					break;
				case UINavigationType.Right:
					delta = 1;
					break;
				case UINavigationType.None:
					return;
			}

			var next = Mathf.Clamp(focusedViewItemIndex + delta, 0, Enum.GetValues(typeof(Member)).Length);
			focusedViewItemIndex = next;
			if (next < filterSize)
			{
				FocusTo(next);
			}
			else
			{
				FocusTo(confirmButtonNavigator);
			}
		}

		public override void OnSubmit()
		{
			//현재 네비게이션 버튼 아이템을 선택
			if (focusedNavigatableItem is VocalFilterSelectableListView focusedListItem)
			{
				list.ToggleViewItemSelection(focusedListItem);
			}
			else
			{
				Confirm();
			}
		}

		public override void OnCancel()
		{
			Cancel();
		}

		private void FocusTo(int index)
		{
			FocusTo(list.listItems.First(item => item.Id == focusedViewItemIndex));
		}

		private void FocusTo(INavigatable navigatable)
		{
			focusedNavigatableItem?.Unfocus();
			focusedNavigatableItem = navigatable;
			focusedNavigatableItem?.Focus();
		}
	}

	public class VocalFilterPopupParameter : PopupParameterBase
	{
		public Member LastFilter;
	}

	public class VocalFilterPopupResponse : PopupResponse<VocalFilterPopupPayload>
	{
	}

	public class VocalFilterPopupPayload : PopupResultPayload
	{
		public Member FilterMember;
	}
}
