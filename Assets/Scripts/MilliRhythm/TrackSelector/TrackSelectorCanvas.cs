using System.Collections.Generic;
using MilliRhythm.Data.GameDataService;
using UnityEngine;

namespace MilliRhythm.TrackSelector
{
	public class TrackSelectorCanvas : MonoBehaviour
	{
		[SerializeField] private TrackSelectorList trackSelectorList;

		public void Set()
		{
			var data = GameDataService.GetAllMusicData();
			var models = new List<TrackSelectorListModel>();
			foreach (var musicData in data)
			{
				var model = new TrackSelectorListModel(musicData.Id);
				model.MiniJacketSprite = musicData.JacketThumbnail;
			}
			trackSelectorList.Set(models);
		}
	}
}
