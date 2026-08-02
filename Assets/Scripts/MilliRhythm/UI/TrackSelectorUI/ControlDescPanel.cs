using System;
using MilliRhythm.Input;
using UnityEngine;

namespace MilliRhythm.UI.TrackSelectorUI
{
	public class ControlDescPanel : MonoBehaviour
	{
		[SerializeField] private GameObject padPanel;
		[SerializeField] private GameObject kmPanel;

		public void OnChangeDevice(InputDeviceType type)
		{
			switch (type)
			{
				case InputDeviceType.KeyboardMouse:
					padPanel.SetActive(false);
					kmPanel.SetActive(true);
					break;
				case InputDeviceType.Mobile:
					padPanel.SetActive(false);
					kmPanel.SetActive(false);
					break;
				case InputDeviceType.Gamepad:
					padPanel.SetActive(true);
					kmPanel.SetActive(false);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}
