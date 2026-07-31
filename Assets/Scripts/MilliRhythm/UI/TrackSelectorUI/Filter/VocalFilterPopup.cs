using System;
using System.Collections.Generic;
using MilliRhythm.Data.Domain;
using MilliRhythm.UI.Components;
using UnityEngine;

namespace MilliRhythm.UI.TrackSelectorUI.Filter
{
	public class VocalFilterPopup : PopupUIBase<VocalFilterPopupParameter, VocalFilterPopupResponse, VocalFilterPopupPayload>
	{
		[SerializeField] private VocalFilterSelectableList list;

		protected override void Set()
		{
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
		}

		public override void OnSubmit(bool value)
		{
			//현재 네비게이션 버튼 아이템을 선택
		}

		public override void OnCancel(bool value)
		{
			Cancel();
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
