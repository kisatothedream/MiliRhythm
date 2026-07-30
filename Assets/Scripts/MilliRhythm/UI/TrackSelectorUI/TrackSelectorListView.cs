using System;
using MilliRhythm.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.TrackSelector
{
	public class TrackSelectorListView : UIListItemViewBase<TrackSelectorListModel, int>
	{
		public bool IsFiltered { get; private set; }
		public int NavigationId { get; private set; }
		[SerializeField] private GameObject selectedMark;
		[SerializeField] private Button button;

		[SerializeField] private Image thumbnailImage;

		//랭크 정보
		[SerializeField] private TextMeshProUGUI trackName;
		private Action<TrackSelectorListModel> onClickButtonAction;

		private void Awake()
		{
			button.onClick.AddListener(OnClickButtonAction);
		}

		private void OnDestroy()
		{
			button.onClick.RemoveListener(OnClickButtonAction);
		}

		public void SetButtonAction(Action<TrackSelectorListModel> action)
		{
			onClickButtonAction = action;
		}

		protected override void ApplyModel(TrackSelectorListModel m)
		{
			Model = m;
			thumbnailImage.sprite = Model.ThumbnailSprite;
			trackName.text = $"{Model.TrackName}";
		}

		public void SetNavigationId(int navigationId)
		{
			NavigationId = navigationId;
		}

		protected override void UpdateUI(bool selected)
		{
			selectedMark.SetActive(selected);
		}

		private void OnClickButtonAction()
		{
			onClickButtonAction?.Invoke(Model);
		}

		public void SetFiltered(bool isFiltered)
		{
			IsFiltered = isFiltered;
			gameObject.SetActive(!isFiltered);
		}
	}
}
