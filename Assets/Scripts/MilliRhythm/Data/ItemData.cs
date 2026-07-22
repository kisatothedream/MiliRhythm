using MilliRhythm.Data.GameDataService;

namespace MilliRhythm.Data
{
	public sealed class ItemData : IGameData<int>
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Price { get; set; }
		public ItemType Type { get; set; }
	}

	public enum ItemType
	{
		Weapon,
		Armor,
		Module,
		Consumable,
	}

	public sealed class ItemDataRepository : CsvDataRepository<int, ItemData>
	{
		protected override string FileName => "ItemData.csv";
	}
}
