using MilliRhythm.Config;
using MilliRhythm.Data.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.ConfigUI
{
	public class ConfigPanel : MonoBehaviour
	{
		[SerializeField] private GameObject panel;

		//Volume
		[SerializeField] private Slider masterVolumeSlider;
		[SerializeField] private Slider musicVolumeSlider;
		[SerializeField] private Slider sfxVolumeSlider;

		//Toggle Group
		[SerializeField] private Toggle japaneseToggle;
		[SerializeField] private Toggle koreanToggle;
		[SerializeField] private Toggle englishToggle;

		[SerializeField] private Button quitButton;

		private void Awake()
		{
			masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
			musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
			sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

			japaneseToggle.onValueChanged.AddListener(selected => OnLanguageSelected(selected, LanguageType.Japanese));
			koreanToggle.onValueChanged.AddListener(selected => OnLanguageSelected(selected, LanguageType.Korean));
			englishToggle.onValueChanged.AddListener(selected => OnLanguageSelected(selected, LanguageType.English));

			quitButton.onClick.AddListener(Quit);
		}

		private void OnDestroy()
		{
			masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
			musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
			sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);

			japaneseToggle.onValueChanged.RemoveListener(selected => OnLanguageSelected(selected, LanguageType.Japanese));
			koreanToggle.onValueChanged.RemoveListener(selected => OnLanguageSelected(selected, LanguageType.Korean));
			englishToggle.onValueChanged.RemoveListener(selected => OnLanguageSelected(selected, LanguageType.English));

			quitButton.onClick.RemoveListener(Quit);
		}

		private void OnEnable()
		{
			Refresh();
		}

		public void Show()
		{
			panel.SetActive(true);
			Refresh();
		}

		private void Quit()
		{
			panel.SetActive(false);
			ConfigManager.Instance.Save();
		}

		private void Refresh()
		{
			var config = ConfigManager.Instance.Config;
			masterVolumeSlider.value = config.MasterVolume;
			musicVolumeSlider.value = config.MusicVolume;
			sfxVolumeSlider.value = config.SfxVolume;

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
	}
}
