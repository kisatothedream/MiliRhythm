namespace MilliRhythm.Data.GameDataService
{
	public interface IGameData<TKey>
	{
		TKey Id { get; }
	}
}
