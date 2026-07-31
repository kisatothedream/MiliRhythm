namespace MilliRhythm.UI.Components
{
	public interface INavigatable
	{
		void Focus();
		void Unfocus();
		abstract void ApplyFocusState(bool focused);
	}
}
