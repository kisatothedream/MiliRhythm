using System.Collections.Generic;
using R3;

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
			this.uiControls = controls;
		}

		public void Register(IUIInputListener inputListener)
		{
			if (popupStack.TryPeek(out var current))
				UnregisterToControls(current);
			popupStack.Push(inputListener);
			RegisterToControls(popupStack.Peek());
		}

		public void Unregister(IUIInputListener inputListener)
		{
			popupStack.Pop();
			UnregisterToControls(inputListener);
		}

		private void RegisterToControls(IUIInputListener inputListener)
		{
			var disposable = new CompositeDisposable();
			disposables.Add(inputListener, disposable);
			uiControls.Navigate.Subscribe(inputListener.OnNavigate).AddTo(disposable);
			uiControls.Submit.Subscribe(inputListener.OnSubmit).AddTo(disposable);
			uiControls.Cancel.Subscribe(inputListener.OnCancel).AddTo(disposable);
		}

		private void UnregisterToControls(IUIInputListener inputListener)
		{
			disposables[inputListener].Dispose();
			disposables.Remove(inputListener);
		}
	}
}
