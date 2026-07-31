using System;
using MilliRhythm.Data.Domain;
using MilliRhythm.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.TrackSelectorUI.Filter
{
	public class VocalFilterSelectableListView : SelectableListViewBase<VocalFilterSelectableListModel, int>, INavigatable
	{
		[SerializeField] private GameObject selectedMark;
		[SerializeField] private Image filterImage;
		[SerializeField] private LocalizedText filterText;


		private void Awake()
		{
			Deselect();
		}

		protected override void ApplyModel(VocalFilterSelectableListModel model)
		{
			filterText.SetLocalizationKey(model.Member.GetMemberNameKey());
		}

		protected override void UpdateUI(bool selected)
		{
			selectedMark.SetActive(selected);
		}

		public void Focus()
		{
		}

		public void Unfocus()
		{
		}

		public void ApplyFocusState(bool focused)
		{
		}
	}

	public class VocalFilterSelectableListModel : SelectableListModelBase<int>
	{
		public Member Member;

		public VocalFilterSelectableListModel(int id, Member member) : base(id)
		{
			Member = member;
		}
	}
}
