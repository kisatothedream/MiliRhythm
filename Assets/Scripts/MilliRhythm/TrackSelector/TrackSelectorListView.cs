using System;
using MilliRhythm.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.TrackSelector
{
	public class TrackSelectorListView: UIListItemViewBase<TrackSelectorListModel, int>
	{
		[SerializeField] private GameObject selectedMark;
		[SerializeField] private Button button;
		[SerializeField] private Image jacketImage;
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
			model = m;
			jacketImage.sprite = model.MiniJacketSprite;
			trackName.text = $"{model.trackName}";
		}

		protected override void UpdateUI(bool selected)
		{
			selectedMark.SetActive(selected);
		}

		private void OnClickButtonAction()
		{
			onClickButtonAction?.Invoke(model);
		}
	}
}
