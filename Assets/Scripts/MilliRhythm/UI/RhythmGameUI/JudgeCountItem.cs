using TMPro;
using UnityEngine;

namespace MilliRhythm.UI.RhythmGameUI
{
	public class JudgeCountItem : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI countText;

		private void Awake()
		{
			gameObject.SetActive(false);
		}

		public void SetCountAndShow(int count)
		{
			countText.text = count.ToString();
			gameObject.SetActive(true);
		}
	}
}
