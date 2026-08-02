using System;
using MilliRhythm.Config;
using MilliRhythm.Data.Domain;
using MilliRhythm.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.ConfigUI
{
	public class ConfigPopup : PopupUIBase<CommonPopupParameter, CommonPopupResponse, CommonPopupResultPayload>
	{
		private INavigatable[] navigatables;
		private INavigatable currentNavigatable;

		//Volume
		[SerializeField] private NavigatableSlider masterVolumeSlider;
		[SerializeField] private NavigatableSlider musicVolumeSlider;
		[SerializeField] private NavigatableSlider sfxVolumeSlider;

		//Toggle Group
		[SerializeField] private Toggle japaneseToggle;
		[SerializeField] private Toggle koreanToggle;
		[SerializeField] private Toggle englishToggle;

		[SerializeField] private Button quitButton;

		private void Awake()
		{
			response = new CommonPopupResponse();

			navigatables = GetComponentsInChildren<INavigatable>(true);
			masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
			musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
			sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

			japaneseToggle.onValueChanged.AddListener(selected => OnLanguageSelected(selected, LanguageType.Japanese));
			koreanToggle.onValueChanged.AddListener(selected => OnLanguageSelected(selected, LanguageType.Korean));
			englishToggle.onValueChanged.AddListener(selected => OnLanguageSelected(selected, LanguageType.English));

			quitButton.onClick.AddListener(Confirm);
			Refresh();
		}

		private void OnDestroy()
		{
			masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
			musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
			sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);

			japaneseToggle.onValueChanged.RemoveListener(selected => OnLanguageSelected(selected, LanguageType.Japanese));
			koreanToggle.onValueChanged.RemoveListener(selected => OnLanguageSelected(selected, LanguageType.Korean));
			englishToggle.onValueChanged.RemoveListener(selected => OnLanguageSelected(selected, LanguageType.English));

			quitButton.onClick.RemoveListener(Confirm);
		}

		protected override void Set()
		{
			Refresh();
			currentNavigatable = navigatables[0];
		}

		private void Refresh()
		{
			var config = ConfigManager.Instance.Config;
			masterVolumeSlider.SetValueWithoutNotify(config.MasterVolume);
			musicVolumeSlider.SetValueWithoutNotify(config.MusicVolume);
			sfxVolumeSlider.SetValueWithoutNotify(config.SfxVolume);

			var language = ConfigManager.Instance.Config.Language;

			koreanToggle.SetIsOnWithoutNotify(language == LanguageType.Korean);
			japaneseToggle.SetIsOnWithoutNotify(language == LanguageType.Japanese);
			englishToggle.SetIsOnWithoutNotify(language == LanguageType.English);
		}

		private void OnMasterVolumeChanged(float volume)
		{
			ConfigManager.Instance.ChangeMasterVolume(volume);
		}

		private void OnMusicVolumeChanged(float volume)
		{
			ConfigManager.Instance.ChangeMusicVolume(volume);
		}

		private void OnSfxVolumeChanged(float volume)
		{
			ConfigManager.Instance.ChangeSfxVolume(volume);
		}

		private void OnLanguageSelected(bool selected, LanguageType type)
		{
			if (!selected) return;
			ConfigManager.Instance.ChangeLanguage(type);
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
		}

		private void Select(int index)
		{
			var next = Math.Clamp(index, 0, navigatables.Length - 1);
			currentNavigatable = navigatables[next];
		}

		public override void OnSubmit(bool value)
		{
			ConfigManager.Instance.Save();
			Confirm();
		}

		public override void OnCancel(bool value)
		{
			Confirm();
			ConfigManager.Instance.Save();
		}
	}
}
