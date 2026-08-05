using UnityEngine;

namespace MilliRhythm.UI
{
	public enum UINavigationType
	{
		None,
		Up,
		Down,
		Left,
		Right,
	}

	public static class UINavigationTypeExtensions
	{
		public static UINavigationType ToUINavigationType(this Vector2 value)
		{
			if (value.x > 0.5f)
			{
				return UINavigationType.Right;
			}
			else if (value.x < -0.5f)
			{
				return UINavigationType.Left;
			}
			else if (value.y > 0.5f)
			{
				return UINavigationType.Up;
			}
			else if (value.y < -0.5f)
			{
				return UINavigationType.Down;
			}

			return UINavigationType.None;
		}
	}
}
