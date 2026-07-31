using System;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.Components
{
	public abstract class NavigateButtonBase : MonoBehaviour
	{
		[SerializeField] private Button button;
		public bool IsFocused { get; private set; }

		private void Awake()
		{
			button.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
			button.onClick.RemoveListener(OnClick);
		}

		public void Focus()
		{
			if (IsFocused)
				return;

			IsFocused = true;
			ApplyFocusState(true);
		}

		public void Unfocus()
		{
			if (!IsFocused)
				return;

			IsFocused = false;
			ApplyFocusState(false);
		}

		public abstract void OnClick();

		protected abstract void ApplyFocusState(bool focused);
	}
}
