using System;

namespace MilliRhythm.Data.Domain
{
	[Flags]
	public enum Member
	{
		AkubiDemonspade = 1 << 0,
		AmakamiKonomi = 1 << 1,
		AmayoLiz = 1 << 2,
		KomawariKoma = 1 << 3,
		NemukumoTsukuri = 1 << 4,
		NijipukaNuhu = 1 << 5,
		NononoNono = 1 << 6,
		OtonoseRaco = 1 << 7,
		YugiriRay = 1 << 8,
		YuragiYura = 1 << 9,
	}

	public static class MemberExtensions
	{
		public static string GetMemberNameKey(this Member member)
		{
			return "temp";
			// return member switch
			// {
			// 	// Member.AkubiDemonspade => ,
			// 	// Member.AmakamiKonomi => expr,
			// 	// Member.AmayoLiz => expr,
			// 	// Member.KomawariKoma => expr,
			// 	// Member.NemukumoTsukuri => expr,
			// 	// Member.NijipukaNuhu => expr,
			// 	// Member.NononoNono => expr,
			// 	// Member.OtonoseRako => expr,
			// 	// Member.YugiriRay => expr,
			// 	// Member.YuragiYura => expr,
			// 	// _ => throw new ArgumentOutOfRangeException(nameof(member), member, null)
			// };
		}
	}
}
