using System.IO;
using UnityEngine;

namespace MilliRhythm.User.Save
{
	public static class SaveManager
	{
		private const string SaveFileName = "user_save.json";

		private static string SavePath =>
			Path.Combine(Application.persistentDataPath, SaveFileName);

		public static void Save(UserSaveData saveData)
		{
			var json = JsonUtility.ToJson(saveData, true);
			File.WriteAllText(SavePath, json);
			Debug.Log(SavePath);
		}

		public static bool TryLoad(out UserSaveData saveData)
		{
			saveData = null;
			if (!File.Exists(SavePath))
			{
				return false;
			}

			var json = File.ReadAllText(SavePath);

			if (string.IsNullOrEmpty(json)) return false;
			saveData = JsonUtility.FromJson<UserSaveData>(json);

			return saveData != null;
		}

		public static void DeleteSave()
		{
			if (File.Exists(SavePath))
			{
				File.Delete(SavePath);
			}
		}
	}
}
