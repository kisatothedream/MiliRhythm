using MilliRhythm.Data.Domain;
using MilliRhythm.Data.GameDataService;
using MilliRhythm.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.TrackSelectorUI.Filter
{
	public class VocalFilterSelectableListView : SelectableListViewBase<VocalFilterSelectableListModel, int>, INavigatable
	{
		[SerializeField] private GameObject focusMark;
		[SerializeField] private GameObject selectedMark;
		[SerializeField] private Image filterImage;
		[SerializeField] private LocalizedText filterText;


		private void Awake()
		{
			Deselect();
			Unfocus();
		}

		protected override void ApplyModel(VocalFilterSelectableListModel model)
		{
			filterText.SetLocalizationKey(model.Member.GetMemberShortNameKey());
			filterImage.sprite = GameDataService.GetMemberData(model.Member).Icon;
		}

		protected override void UpdateUI(bool selected)
		{
			selectedMark.SetActive(selected);
		}

		public void Focus()
		{
			ApplyFocusState(true);
		}

		public void Unfocus()
		{
			ApplyFocusState(false);
		}

		public void ApplyFocusState(bool focused)
		{
			focusMark.SetActive(focused);
		}

		public void OnNavigate(UINavigationType direction)
		{
		}

		public void OnSubmit()
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
