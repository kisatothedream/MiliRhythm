using System;
using MilliRhythm.Data.Domain;
using MilliRhythm.Data.GameDataService;
using MilliRhythm.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.TrackSelectorUI.StartPopup
{
	public class GameStartPopup : PopupUIBase<GameStartPopupParameter, GameStartPopupResponse, GameStartPopupPayload>
	{
		private INavigatable[] navigatables;
		private INavigatable currentNavigatable;
		[SerializeField] private Toggle melodyToggle;
		[SerializeField] private Toggle beatToggle;
		[SerializeField] private ChartTypeTogglableUI melodyToggleUI;
		[SerializeField] private ChartTypeTogglableUI beatToggleUI;

		[SerializeField] private Image jacketImage;
		[SerializeField] private LocalizedText trackNameText;
		[SerializeField] private LocalizedText vocalText;

		[SerializeField] private TextMeshProUGUI melodyScoreText;
		[SerializeField] private TextMeshProUGUI beatScoreText;
		[SerializeField] private TextMeshProUGUI melodyRankText;
		[SerializeField] private TextMeshProUGUI beatRankText;

		protected override void OnAwake()
		{
			melodyToggle.onValueChanged.AddListener(OnMelodyToggleChanged);
			beatToggle.onValueChanged.AddListener(OnBeatToggleChanged);
		}

		protected override void OnDestroyNested()
		{
			melodyToggle.onValueChanged.RemoveListener(OnMelodyToggleChanged);
			beatToggle.onValueChanged.RemoveListener(OnBeatToggleChanged);
		}

		protected override void Set()
		{
			navigatables = GetComponentsInChildren<INavigatable>();
			response = new GameStartPopupResponse();
			response.Payload = new GameStartPopupPayload
			{
				TrackId = parameter.Model.Id,
			};
			jacketImage.sprite = parameter.Model.ThumbnailSprite;
			trackNameText.LocalizationKey = parameter.Model.TrackNameKey;
			vocalText.LocalizationKey = parameter.Model.TrackVocal.GetMemberNameKey();
			melodyScoreText.text = parameter.MelodyScore.ToString();
			beatScoreText.text = parameter.BeatScore.ToString();

			Select(0);
		}

		public override void OnNavigate(Vector2 value)
		{
			var direction = value.ToUINavigationType();
			switch (direction)
			{
				case UINavigationType.None:
					break;
				case UINavigationType.Up:
					Select(Array.IndexOf(navigatables, currentNavigatable) - 1);
					break;
				case UINavigationType.Down:
					Select(Array.IndexOf(navigatables, currentNavigatable) + 1);
					break;
				case UINavigationType.Left:
				case UINavigationType.Right:
					currentNavigatable.OnNavigate(direction);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			response.Payload.ChartType = Array.IndexOf(navigatables, currentNavigatable) == 0 ? ChartType.Melody : ChartType.Beat;
		}

		private void Select(int index)
		{
			currentNavigatable?.Unfocus();
			var next = Math.Clamp(index, 0, navigatables.Length - 1);
			currentNavigatable = navigatables[next];
			currentNavigatable.Focus();
		}

		public override void OnSubmit()
		{
			currentNavigatable?.OnSubmit();
		}

		public override void OnCancel()
		{
			Cancel();
		}

		public override void OnView()
		{
		}

		private void OnMelodyToggleChanged(bool selected)
		{
			melodyToggleUI.SetSelected(selected);
		}

		private void OnBeatToggleChanged(bool selected)
		{
			beatToggleUI.SetSelected(selected);
		}
	}

	public class GameStartPopupParameter : PopupParameterBase
	{
		public TrackSelectorListModel Model;

		//곡 정보
		public int MelodyScore;
		public int BeatScore;
		public string MelodyRank;
		public string BeatRank;
		//마지막 실행한 것
	}

	public class GameStartPopupResponse : PopupResponse<GameStartPopupPayload>
	{
	}

	public class GameStartPopupPayload : PopupResultPayload
	{
		public int TrackId;
		public ChartType ChartType;
		public Difficulty Difficulty;
	}
}
