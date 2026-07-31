using MilliRhythm.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.TrackSelectorUI.Filter
{
	public class VocalFilterNavigateButton : NavigateButtonBase
	{
		[SerializeField] private Image memberFaceImage;
		[SerializeField] private TextMeshProUGUI memberName;

		public override void OnClick()
		{
		}

		protected override void ApplyFocusState(bool focused)
		{
		}

		public void Select()
		{
			ApplySelectedState(true);
		}

		public void Deselect()
		{
			ApplySelectedState(false);
		}

		private void ApplySelectedState(bool selected)
		{
		}
	}
}
