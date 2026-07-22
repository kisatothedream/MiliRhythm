using MilliRhythm.Data.GameDataService;
using MilliRhythm.Data.Repository;
using TMPro;
using UnityEngine;

namespace MilliRhythm.UI.Components
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshProUGUI))]
	public sealed class LocalizedText : MonoBehaviour
	{
		[SerializeField] private string localizationKey;

		private TextMeshProUGUI textComponent;

		public string LocalizationKey => localizationKey;

		private void Awake()
		{
			textComponent = GetComponent<TextMeshProUGUI>();
		}

		private void OnEnable()
		{
			Refresh();
		}

		private void OnDisable()
		{
		}

		public void SetLocalizationKey(string key)
		{
			localizationKey = key;
			Refresh();
		}

		public void Refresh()
		{
			if (string.IsNullOrWhiteSpace(localizationKey))
				return;

			textComponent ??= GetComponent<TextMeshProUGUI>();

			if (!GameDataService.Initialized)
			{
				Debug.LogWarning($"GameDataService is not initialized. " + $"Object: {name}, Key: {localizationKey}", this);

				return;
			}

			if (GameDataService.TryGetLocalization(localizationKey, out var data))
			{
				textComponent.text = data.Text;
				return;
			}

			textComponent.text = localizationKey;

			Debug.LogWarning($"Localization key not found. " + $"Object: {name}, Key: {localizationKey}", this);
		}

		private void OnLanguageChanged(LanguageType language)
		{
			Refresh();
		}
	}
}
