using System;
using Cysharp.Threading.Tasks;
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

		[SerializeField] private QuitGamePopup quitGamePopup;
		[SerializeField] private CreditPopup creditPopup;
		[SerializeField] private NavigatableButton quitGameButton;
		[SerializeField] private NavigatableButton creditButton;

		private void Awake()
		{
			response = new CommonPopupResponse();

			navigatables = GetComponentsInChildren<INavigatable>(true);
			masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
			musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
			sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

			japaneseToggle.onValueChanged.AddListener(OnJaSelected);
			koreanToggle.onValueChanged.AddListener(OnKoSelected);
			englishToggle.onValueChanged.AddListener(OnEnSelected);

			quitGameButton.onClick.AddListener(DisplayQuitGamePopup);
			creditButton.onClick.AddListener(DisplayConfigGamePopup);

			Refresh();
		}

		private void OnDestroy()
		{
			masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
			musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
			sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);

			japaneseToggle.onValueChanged.RemoveListener(OnJaSelected);
			koreanToggle.onValueChanged.RemoveListener(OnKoSelected);
			englishToggle.onValueChanged.RemoveListener(OnEnSelected);

			quitGameButton.onClick.RemoveListener(DisplayQuitGamePopup);
			creditButton.onClick.RemoveListener(DisplayConfigGamePopup);
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

		private void OnJaSelected(bool selected) => OnLanguageSelected(selected, LanguageType.Japanese);
		private void OnKoSelected(bool selected) => OnLanguageSelected(selected, LanguageType.Korean);
		private void OnEnSelected(bool selected) => OnLanguageSelected(selected, LanguageType.English);

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
			if (!value) return;
			ConfigManager.Instance.Save();
			currentNavigatable.OnSubmit();
		}

		public override void OnCancel(bool value)
		{
			if (!value) return;
			Confirm();
			ConfigManager.Instance.Save();
		}

		public override void OnView(bool value)
		{
		}

		public void DisplayConfigGamePopup()
		{
			UniTask.Action(async () => { await creditPopup.Display(null); });
		}

		public void DisplayQuitGamePopup()
		{
			UniTask.Action(async () =>
			{
				var result = await quitGamePopup.Display(null);
				if (result.Result == PopupResult.Confirm)
				{
					Application.Quit();
				}
			});
		}
	}
}
