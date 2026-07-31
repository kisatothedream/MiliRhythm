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

		private TPopupResponse response;
		private CancellationTokenSource cts;

		private void Awake()
		{
			confirmButton.onClick.AddListener(Confirm);
			cancelButton.onClick.AddListener(Cancel);
			dismissArea.onClick.AddListener(Dismiss);
		}

		private void OnDestroy()
		{
			confirmButton.onClick.RemoveListener(Confirm);
			cancelButton.onClick.RemoveListener(Cancel);
			dismissArea.onClick.RemoveListener(Dismiss);
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

		protected void Confirm()
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

		public abstract void OnNavigate(Vector2 value);
		public abstract void OnSubmit(bool value);
		public abstract void OnCancel(bool value);
	}


	public enum PopupResult
	{
		Confirm,
		Cancel,
		Dismiss,
	}

	public abstract class PopupParameterBase
	{
		public bool CanDismiss;
	}

	public class CommonPopupResponse : PopupResponse<CommonPopupResultPayload>
	{
	}

	public abstract class PopupResponse<T> where T : PopupResultPayload
	{
		public PopupResult Result;
		public T Payload;
	}

	public class CommonPopupResultPayload : PopupResultPayload
	{
	}

	public abstract class PopupResultPayload
	{
	}
}
