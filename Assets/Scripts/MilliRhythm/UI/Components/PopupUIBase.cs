using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MilliRhythm.Input;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.Components
{
	public abstract class PopupUIBase<TPopupParameter, TPopupResponse, TPopupPayload> : MonoBehaviour, IUIInputListener
		where TPopupParameter : PopupParameterBase
		where TPopupPayload : PopupResultPayload
		where TPopupResponse : PopupResponse<TPopupPayload>
	{
		[SerializeField] private Button confirmButton;
		[SerializeField] private Button cancelButton;
		[SerializeField] private Button dismissArea;
		protected TPopupParameter parameter;

		protected TPopupResponse response;
		private CancellationTokenSource cts;

		private void Awake()
		{
			OnAwake();
			confirmButton?.onClick.AddListener(Confirm);
			cancelButton?.onClick.AddListener(Cancel);
			dismissArea?.onClick.AddListener(Dismiss);
		}

		protected virtual void OnAwake()
		{
		}

		private void OnDestroy()
		{
			OnDestroyNested();
			confirmButton?.onClick.RemoveListener(Confirm);
			cancelButton?.onClick.RemoveListener(Cancel);
			dismissArea?.onClick.RemoveListener(Dismiss);
		}

		protected virtual void OnDestroyNested()
		{
		}

		public async UniTask<TPopupResponse> Display(TPopupParameter param)
		{
			try
			{
				this.RegisterUIInputListener();
				gameObject.SetActive(true);
				parameter = param;
				Set();
				cts = new CancellationTokenSource();
				await UniTask.WaitUntilCanceled(cts.Token);
				gameObject.SetActive(false);
				return response;
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				throw;
			}
			finally
			{
				this.UnregisterUIInputListener();
			}
		}

		protected abstract void Set();

		private void DisposeToken()
		{
			cts.Cancel();
			cts.Dispose();
			cts = null;
		}

		protected virtual void Confirm()
		{
			response.Result = PopupResult.Confirm;
			DisposeToken();
		}

		protected void Cancel()
		{
			response.Result = PopupResult.Cancel;
			DisposeToken();
		}

		private void Dismiss()
		{
			if (!parameter.CanDismiss) return;
			response.Result = PopupResult.Dismiss;
			DisposeToken();
		}

		public void Navigate(Vector2 value)
		{
			OnNavigate(value);
		}

		public abstract void OnNavigate(Vector2 value);

		public void Submit(bool value)
		{
			if (!value) return;
			OnSubmit();
		}

		public abstract void OnSubmit();

		public void Cancel(bool value)
		{
			if (!value) return;
			OnCancel();
		}

		public void Config(bool value)
		{
		}

		public void Filter(bool value)
		{
		}

		public void AnyKey(bool value)
		{
		}

		public abstract void OnCancel();
	}


	public enum PopupResult
	{
		Confirm,
		Cancel,
		Dismiss,
	}

	public class DefaultPopupParameter : PopupParameterBase
	{
	}

	public abstract class PopupParameterBase
	{
		public bool CanDismiss = false;
	}

	public class DefaultPopupResponse : PopupResponse<DefaultPopupResultPayload>
	{
	}

	public abstract class PopupResponse<T> where T : PopupResultPayload
	{
		public PopupResult Result;
		public T Payload;
	}

	public class DefaultPopupResultPayload : PopupResultPayload
	{
	}

	public abstract class PopupResultPayload
	{
	}
}
