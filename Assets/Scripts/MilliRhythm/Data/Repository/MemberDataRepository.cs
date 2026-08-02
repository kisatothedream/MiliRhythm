using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MilliRhythm.Data.Domain;
using MilliRhythm.Data.GameDataService;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MilliRhythm.Data.Repository
{
	public class MemberDataRepository : IGameDataRepository
	{
		private AsyncOperationHandle<MemberDataCollection> handle;
		private MemberDataCollection collection;
		private const string FileKey = "MemberDataRepository";
		private List<MemberData> memberDataList;
		private readonly Dictionary<Member, MemberData> memberDataMap = new();

		public async UniTask LoadAsync()
		{
			handle = Addressables.LoadAssetAsync<MemberDataCollection>(FileKey);
			collection = await handle.Task;
			memberDataList = new List<MemberData>(collection.MemberDataList);
			foreach (var memberData in memberDataList)
			{
				memberDataMap.Add(memberData.MemberType, memberData);
			}
		}

		public void Release()
		{
			if (handle.IsValid())
			{
				Addressables.Release(handle);
			}
		}

		public MemberData GetMemberData(Member member)
		{
			if (!memberDataMap.TryGetValue(member, out var data))
			{
				throw new KeyNotFoundException($"Member {member} is not registered.");
			}

			return data;
		}
	}
}
