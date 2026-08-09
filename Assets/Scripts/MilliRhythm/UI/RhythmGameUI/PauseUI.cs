using System;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.Domain;
using MilliRhythm.Input;
using MilliRhythm.UI.Components;
using MilliRhythm.UI.ConfigUI;
using UnityEngine;

namespace MilliRhythm.UI.RhythmGameUI
{
	public struct TrackContext
	{
		public int TrackId;
		public ChartType ChartType;
	}

	public class PauseUI : MonoBehaviour, IUIInputListener
	{
		private INavigatable[] navigatables;
		private INavigatable currentNavigatable;
		[SerializeField] private NavigatableButton resumeButton;
		[SerializeField] private NavigatableButton restartButton;
		[SerializeField] private NavigatableButton configButton;
		[SerializeField] private ConfigPopup configPopup;
		[SerializeField] private CommonPopup commonPopup;
		private TrackContext context;
		private bool isPopupOpened;

		private void Awake()
		{
			navigatables = GetComponentsInChildren<INavigatable>(true);
			resumeButton.onClick.AddListener(Hide);
			restartButton.onClick.AddListener(DisplayRestartPopup);
			configButton.onClick.AddListener(DisplayConfigPopup);

			Select(0);
		}

		private void OnDestroy()
		{
			resumeButton.onClick.RemoveListener(Hide);
			restartButton.onClick.RemoveListener(DisplayRestartPopup);
			configButton.onClick.RemoveListener(DisplayConfigPopup);
		}

		private void OnEnable()
		{
			this.RegisterUIInputListener();
		}

		private void OnDisable()
		{
			this.UnregisterUIInputListener();
		}

		public void Display(TrackContext ctx)
		{
			context = ctx;
			gameObject.SetActive(true);
		}

		public void Hide() => gameObject.SetActive(false);


		public void Navigate(Vector2 value)
		{
			OnNavigate(value);
		}

		//TODO: IInputListener 공통 기능으로 뺄 수 있게 재설계..
		private void OnNavigate(Vector2 value)
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

		public void Submit(bool value)
		{
			currentNavigatable.OnSubmit();
		}

		public void Cancel(bool value)
		{
		}

		public void View(bool value)
		{
		}

		private void DisplayRestartPopup()
		{
			if (isPopupOpened) return;
			DisplayRestartPopupAsync().Forget();
			return;

			async UniTask DisplayRestartPopupAsync()
			{
				isPopupOpened = true;
				await commonPopup.Display(new CommonPopupParameter()
				{
					TitleTextKey = "TITLE-TEXT-KEY",
					ContentTextKey = "CONTENT-TEXT-KEY",
				});
				isPopupOpened = false;
			}
		}

		private void DisplayConfigPopup()
		{
			if (isPopupOpened) return;
			DisplayConfigPopupAsync().Forget();
			return;

			async UniTask DisplayConfigPopupAsync()
			{
				isPopupOpened = true;
				await configPopup.Display(null);
				isPopupOpened = false;
			}
		}
	}
}
