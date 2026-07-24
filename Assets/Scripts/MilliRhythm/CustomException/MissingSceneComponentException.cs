using System;

namespace MilliRhythm.CustomException
{
	public sealed class MissingSceneComponentException : System.Exception
	{
		public MissingSceneComponentException(string sceneName, Type componentType) : base(CreateMessage(sceneName, componentType))
		{
		}

		private static string CreateMessage(string sceneName, Type componentType)
		{
			return $"Required component '{componentType.Name}' was not found in scene '{sceneName}'.";
		}
	}
}
