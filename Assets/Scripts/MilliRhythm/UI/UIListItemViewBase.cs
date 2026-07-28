using System;
using UnityEngine;

namespace MilliRhythm.UI
{
	public abstract class UIListItemViewBase<TModel, TId> : MonoBehaviour where TModel : UIListItemModelBase<TId> where TId : IEquatable<TId>
	{
		public TId Id => model.Id;
		protected TModel model;

		public virtual void Set(TModel m)
		{
			model = m;
			ApplyModel(m);
		}

		protected abstract void ApplyModel(TModel model);
		protected abstract void UpdateUI(bool selected);
	}
}
