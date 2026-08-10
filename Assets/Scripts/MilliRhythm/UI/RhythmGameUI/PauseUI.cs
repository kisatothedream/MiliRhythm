using System;
using Cysharp.Threading.Tasks;
using MilliRhythm.Input;
using MilliRhythm.UI.Components;
using MilliRhythm.UI.ConfigUI;
using UnityEngine;

namespace MilliRhythm.UI.RhythmGameUI
{
	public class PauseUI : MonoBehaviour, IUIInputListener
	{
		private INavigatable[] navigatables;
		private INavigatable currentNavigatable;
		[SerializeField] private NavigatableButton resumeButton;
		[SerializeField] private NavigatableButton restartButton;
		[SerializeField] private NavigatableButton configButton;
		[SerializeField] private NavigatableButton quitButton;
		[SerializeField] private ConfigPopup configPopup;
		[SerializeField] private CommonPopup commonPopup;
		private bool isPopupOpened;
		private Action restartAction;
		private Action quitAction;

		private void Awake()
		{
			navigatables = GetComponentsInChildren<INavigatable>(true);
			resumeButton.onClick.AddListener(Hide);
			restartButton.onClick.AddListener(DisplayRestartPopup);
			configButton.onClick.AddListener(DisplayConfigPopup);
			quitButton.onClick.AddListener(DisplayQuitPopup);
		}

		private void OnDestroy()
		{
			resumeButton.onClick.RemoveListener(Hide);
			restartButton.onClick.RemoveListener(DisplayRestartPopup);
			configButton.onClick.RemoveListener(DisplayConfigPopup);
			quitButton.onClick.RemoveListener(DisplayQuitPopup);
		}

		private void OnEnable()
		{
			this.RegisterUIInputListener();
		}

		private void OnDisable()
		{
			this.UnregisterUIInputListener();
		}

		public void Display(Action restartAction, Action quitAction)
		{
			this.restartAction = restartAction;
			this.quitAction = quitAction;
			gameObject.SetActive(true);
			Select(0);
		}

		private void Hide()
		{
			gameObject.SetActive(false);
		}


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
			if (value)
				currentNavigatable.OnSubmit();
		}

		public void Cancel(bool value)
		{
		}

		public void Config(bool value)
		{
		}

		public void Filter(bool value)
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
				var result = await commonPopup.Display(new CommonPopupParameter()
				{
					TitleTextKey = "TITLE-TEXT-KEY",
					ContentTextKey = "CONTENT-TEXT-KEY",
				});
				isPopupOpened = false;
				if (result.Result == PopupResult.Confirm)
				{
					restartAction?.Invoke();
				}
			}
		}

		private void DisplayQuitPopup()
		{
			if (isPopupOpened) return;
			DisplayQuitPopupAsync().Forget();
			return;

			async UniTask DisplayQuitPopupAsync()
			{
				isPopupOpened = true;
				var result = await commonPopup.Display(new CommonPopupParameter()
				{
					TitleTextKey = "TITLE-TEXT-KEY",
					ContentTextKey = "CONTENT-TEXT-KEY",
				});
				isPopupOpened = false;
				if (result.Result == PopupResult.Confirm)
				{
					quitAction?.Invoke();
				}
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
