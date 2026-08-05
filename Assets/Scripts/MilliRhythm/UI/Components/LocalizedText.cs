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
		public string LocalizationKey
		{
			get => localizationKey;
			set
			{
				localizationKey = value;
				Refresh();
			}
		}

		[SerializeField] private string localizationKey;

		private TextMeshProUGUI textComponent;

		private void Awake()
		{
			textComponent = GetComponent<TextMeshProUGUI>();
		}

		private void OnEnable()
		{
			L10N.OnLanguageChanged += OnLanguageChanged;
			Refresh();
		}

		private void OnDisable()
		{
			L10N.OnLanguageChanged -= OnLanguageChanged;
		}

		public void SetLocalizationKey(string key)
		{
			localizationKey = key;
			Refresh();
		}

		public void Refresh()
		{
			if (string.IsNullOrWhiteSpace(localizationKey))
			{
				textComponent.text = "";
				Debug.LogWarning($"Localization key is empty", this);
				return;
			}

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

		private void OnLanguageChanged()
		{
			Refresh();
		}
	}
}
