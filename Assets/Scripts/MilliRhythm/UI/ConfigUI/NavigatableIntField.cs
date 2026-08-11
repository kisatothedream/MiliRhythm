using System;
using MilliRhythm.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.ConfigUI
{
	public class NavigatableIntField : MonoBehaviour, INavigatable
	{
		public RectTransform RectTransform => transform as RectTransform;

		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private Button increaseButton;
		[SerializeField] private Button decreaseButton;
		[SerializeField] private int unit = 1;
		public event Action<int> OnValueChanged;

		public int FieldValue
		{
			get => fieldValue;
			set
			{
				fieldValue = value;
				OnValueChange(value);
			}
		}

		private int fieldValue;

		private void Awake()
		{
			inputField.onEndEdit.AddListener(SetValue);
			increaseButton.onClick.AddListener(IncreaseValue);
			decreaseButton.onClick.AddListener(DecreaseValue);
		}

		private void OnDestroy()
		{
			inputField.onEndEdit.RemoveListener(SetValue);
			increaseButton.onClick.RemoveListener(IncreaseValue);
			decreaseButton.onClick.RemoveListener(DecreaseValue);
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
			transform.localScale = Vector2.one * (focused ? 1.1f : 1.0f);
		}

		public void SetInitialValue(int value)
		{
			FieldValue = value;
		}

		private void SetValue(string value)
		{
			if (int.TryParse(value, out var intValue))
			{
				fieldValue = intValue;
				OnValueChanged?.Invoke(fieldValue);
			}
			else
			{
				inputField.SetTextWithoutNotify(fieldValue.ToString());
			}
		}

		private void OnValueChange(int value)
		{
			inputField.SetTextWithoutNotify(fieldValue.ToString());
			OnValueChanged?.Invoke(value);
		}

		public void OnNavigate(UINavigationType direction)
		{
			switch (direction)
			{
				case UINavigationType.Left:
					DecreaseValue();
					break;
				case UINavigationType.Right:
					IncreaseValue();
					break;
			}
		}

		private void IncreaseValue() => FieldValue += unit;
		private void DecreaseValue() => FieldValue -= unit;

		public void OnSubmit()
		{
		}
	}
}
