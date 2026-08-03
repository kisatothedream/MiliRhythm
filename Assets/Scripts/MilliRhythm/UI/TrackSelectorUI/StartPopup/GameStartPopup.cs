using System;
using MilliRhythm.Rhythm;
using MilliRhythm.UI.Components;
using UnityEngine;

namespace MilliRhythm.UI.TrackSelectorUI.StartPopup
{
	public class GameStartPopup : PopupUIBase<GameStartPopupParameter, GameStartPopupResponse, GameStartPopupPayload>
	{
		private INavigatable[] navigatables;
		private INavigatable currentNavigatable;

		protected override void Set()
		{
			navigatables = GetComponentsInChildren<INavigatable>();
			response = new GameStartPopupResponse();
			response.Payload = new GameStartPopupPayload
			{
				TrackId = parameter.Model.Id,
			};
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
			var next = Math.Clamp(index, 0, navigatables.Length - 1);
			currentNavigatable = navigatables[next];
		}

		public override void OnSubmit(bool value)
		{
			Confirm();
		}

		public override void OnCancel(bool value)
		{
			Cancel();
		}

		public override void OnView(bool value)
		{
		}
	}

	public class GameStartPopupParameter : PopupParameterBase
	{
		public TrackSelectorListModel Model;
		//곡 정보
		//ex 점수
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
