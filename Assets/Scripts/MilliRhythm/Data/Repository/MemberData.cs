using MilliRhythm.Data.Domain;
using UnityEngine;

namespace MilliRhythm.Data.Repository
{
	[CreateAssetMenu(fileName = "new Member", menuName = "MilliRhythm/Data/Member")]
	public class MemberData : ScriptableObject
	{
		public Member MemberType;
		public string NameJp;
		public string NameKey;
		public Sprite Icon;
	}
}
