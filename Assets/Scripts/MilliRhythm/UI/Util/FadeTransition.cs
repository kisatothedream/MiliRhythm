using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MilliRhythm.UI.Util
{
	public class FadeTransition : MonoBehaviour
	{
		public static FadeTransition Instance => instance;
		private static FadeTransition instance;

		public Image blackScreen;

		public bool Initialized;

		private void Awake()
		{
			if (instance != null & instance != this)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;
			DontDestroyOnLoad(gameObject);
			Initialized = true;
		}

		public async UniTask FadeOutAsync()
		{
			var alpha = 0f;
			while (alpha < 1)
			{
				alpha += 2.5f * Time.deltaTime;
				blackScreen.color = new Color(0, 0, 0, alpha);

				await UniTask.NextFrame();
			}

			blackScreen.color = new Color(0, 0, 0, 1);
		}

		public async UniTask FadeInAsync()
		{
			var alpha = 1f;
			while (alpha > 0)
			{
				alpha -= 2.5f * Time.deltaTime;
				blackScreen.color = new Color(0,0,0, alpha);

				await UniTask.NextFrame();
			}

			blackScreen.color = new Color(0,0,0, 0);
		}
	}
}
