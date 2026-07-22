using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;

namespace MilliRhythm.Data.GameDataService
{
	public abstract class CsvDataRepository<TKey, TData> : IGameDataRepository where TData : IGameData<TKey>
	{
		private readonly Dictionary<TKey, TData> dataById = new();

		protected abstract string FileName { get; }

		public IReadOnlyDictionary<TKey, TData> All => dataById;

		public virtual async UniTask LoadAsync()
		{
			var loadedData = await CsvDataLoader.LoadAsync<TData>(FileName);

			dataById.Clear();

			foreach (var data in loadedData)
			{
				if (!dataById.TryAdd(data.Id, data))
				{
					throw new InvalidDataException($"Duplicate data ID. DataType: {typeof(TData).Name}, Id: {data.Id}");
				}
			}
		}

		public TData Get(TKey id)
		{
			if (!dataById.TryGetValue(id, out var data))
			{
				throw new KeyNotFoundException($"Data not found. DataType: {typeof(TData).Name}, Id: {id}");
			}

			return data;
		}

		public bool TryGet(TKey id, out TData data)
		{
			return dataById.TryGetValue(id, out data);
		}
	}
}
