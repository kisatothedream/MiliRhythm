using System;
using Cysharp.Threading.Tasks;
using MilliRhythm.Config;
using MilliRhythm.Data.Domain;
using MilliRhythm.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.ConfigUI
{
	public class ConfigPopup : PopupUIBase<DefaultPopupParameter, DefaultPopupResponse, DefaultPopupResultPayload>
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

		[SerializeField] private NavigatableIntField judgeOffset;

		[SerializeField] private Toggle wasdToggle;
		[SerializeField] private Toggle sdklToggle;

		private bool isPopupOpened;

		protected override void OnAwake()
		{
			response = new DefaultPopupResponse();

			navigatables = GetComponentsInChildren<INavigatable>(true);
			masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
			musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
			sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

			japaneseToggle.onValueChanged.AddListener(OnJaSelected);
			koreanToggle.onValueChanged.AddListener(OnKoSelected);
			englishToggle.onValueChanged.AddListener(OnEnSelected);

			quitGameButton.onClick.AddListener(DisplayQuitGamePopup);
			creditButton.onClick.AddListener(DisplayConfigGamePopup);

			wasdToggle.onValueChanged.AddListener(OnWasdSelected);
			sdklToggle.onValueChanged.AddListener(OnSdklSelected);

			judgeOffset.OnValueChanged += OnChangeJudgeOffset;

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

			wasdToggle.onValueChanged.RemoveListener(OnWasdSelected);
			sdklToggle.onValueChanged.RemoveListener(OnSdklSelected);

			judgeOffset.OnValueChanged -= OnChangeJudgeOffset;
		}

		protected override void Set()
		{
			Refresh();
			Select(0);
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

			judgeOffset.SetInitialValue(ConfigManager.Instance.Config.JudgeOffset);

			wasdToggle.SetIsOnWithoutNotify(ConfigManager.Instance.Config.KeyLayout == KeyLayout.WASD);
			sdklToggle.SetIsOnWithoutNotify(ConfigManager.Instance.Config.KeyLayout == KeyLayout.SDKL);
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

		private void OnWasdSelected(bool selected) => OnKeyLayoutChanged(selected, KeyLayout.WASD);
		private void OnSdklSelected(bool selected) => OnKeyLayoutChanged(selected, KeyLayout.SDKL);

		private void OnKeyLayoutChanged(bool selected, KeyLayout layout)
		{
			if (!selected) return;
			ConfigManager.Instance.ChangeKeyLayout(layout);
		}


		private void OnChangeJudgeOffset(int offset)
		{
			ConfigManager.Instance.ChangeJudgeOffset(offset);
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
			currentNavigatable?.Unfocus();
			var next = Math.Clamp(index, 0, navigatables.Length - 1);
			currentNavigatable = navigatables[next];
			currentNavigatable?.Focus();
		}

		public override void OnSubmit()
		{
			ConfigManager.Instance.Save();
			currentNavigatable.OnSubmit();
		}

		public override void OnCancel()
		{
			Confirm();
			ConfigManager.Instance.Save();
		}

		private void DisplayConfigGamePopup()
		{
			if (isPopupOpened) return;
			UniTask.Action(async () =>
			{
				isPopupOpened = true;
				await creditPopup.Display(null);
				isPopupOpened = false;
			})();
		}

		public void DisplayQuitGamePopup()
		{
			if (isPopupOpened) return;
			UniTask.Action(async () =>
			{
				isPopupOpened = true;
				var result = await quitGamePopup.Display(null);
				if (result.Result == PopupResult.Confirm)
				{
					Application.Quit();
				}

				isPopupOpened = false;
			})();
		}
	}
}
