using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace MilliRhythm.Input
{
	public class UIInputManager
	{
		public static UIInputManager Instance => instance ??= new UIInputManager();
		private static UIInputManager instance;
		private readonly Stack<IUIInputListener> popupStack = new();
		private UIControls uiControls;
		private readonly Dictionary<IUIInputListener, CompositeDisposable> disposables = new();

		private UIInputManager()
		{
		}

		public void Init(UIControls controls)
		{
			uiControls = controls;
			var disposable = new CompositeDisposable();
			uiControls.Navigate.Subscribe(OnNavigate).AddTo(disposable);
			uiControls.Submit.Subscribe(OnSubmit).AddTo(disposable);
			uiControls.Cancel.Subscribe(OnCancel).AddTo(disposable);
			uiControls.Config.Subscribe(OnConfig).AddTo(disposable);
			uiControls.Filter.Subscribe(OnFilter).AddTo(disposable);
		}

		public void Register(IUIInputListener inputListener)
		{
			popupStack.Push(inputListener);
		}

		public void Unregister(IUIInputListener inputListener)
		{
			if (popupStack.TryPeek(out var popup))
			{
				if (popup == inputListener)
				{
					popupStack.Pop();
				}
				else
				{
					throw new ArgumentException($"UIInputListener try to pop the other. Check popup register/unregister pair", nameof(inputListener));
				}
			}
		}

		private void OnNavigate(Vector2 direction)
		{
			if (popupStack.TryPeek(out var popup))
			{
				popup.Navigate(direction);
			}
		}

		private void OnSubmit(bool pressed)
		{
			if (popupStack.TryPeek(out var popup))
			{
				popup.Submit(pressed);
			}
		}

		private void OnCancel(bool pressed)
		{
			if (popupStack.TryPeek(out var popup))
			{
				popup.Cancel(pressed);
			}
		}

		private void OnConfig(bool pressed)
		{
			if (popupStack.TryPeek(out var popup))
			{
				popup.Config(pressed);
			}
		}

		private void OnFilter(bool pressed)
		{
			if (popupStack.TryPeek(out var popup))
			{
				popup.Filter(pressed);
			}
		}
	}
}
