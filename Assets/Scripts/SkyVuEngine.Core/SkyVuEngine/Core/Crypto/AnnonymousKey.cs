namespace SkyVuEngine.Core.Crypto
{
	public static class AnnonymousKey
	{
		public static byte[] Key = new byte[32]
		{
			183, 217, 15, 11, 34, 26, 88, 45, 114, 184,
			27, 162, 37, 212, 222, 109, 241, 24, 195, 144,
			173, 53, 196, 29, 214, 26, 17, 218, 131, 236,
			53, 239
		};

		public static byte[] Vector = new byte[16]
		{
			126, 64, 191, 111, 23, 3, 113, 119, 231, 121,
			221, 182, 29, 12, 214, 156
		};
	}
}
