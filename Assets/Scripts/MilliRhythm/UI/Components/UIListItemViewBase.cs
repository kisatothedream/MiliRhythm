using System;
using UnityEngine;

namespace MilliRhythm.UI.Components
{
	public abstract class UIListItemViewBase<TModel, TId> : MonoBehaviour where TModel : UIListItemModelBase<TId> where TId : IEquatable<TId>
	{
		public TId Id => Model.Id;
		public TModel Model { get; protected set; }

		public virtual void Set(TModel m)
		{
			Model = m;
			ApplyModel(m);
		}

		protected abstract void ApplyModel(TModel model);
		protected abstract void UpdateUI(bool selected);
	}
}
