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
				case InputDeviceType.Keyboard:
					padPanel.SetActive(false);
					kmPanel.SetActive(true);
					break;
				case InputDeviceType.Gamepad:
					padPanel.SetActive(true);
					kmPanel.SetActive(false);
					break;
			}
		}
	}
}
