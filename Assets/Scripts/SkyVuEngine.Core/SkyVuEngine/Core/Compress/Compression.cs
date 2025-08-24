using System;
using System.IO;
using SevenZip.Compression.LZMA;

namespace SkyVuEngine.Core.Compress
{
	public class Compression
	{
		public static void CompressFileLZMA(string inFile, string outFile)
		{
			Encoder encoder = new Encoder();
			using (FileStream inFile2 = new FileStream(inFile, FileMode.Open))
			{
				using (FileStream outFile2 = new FileStream(outFile, FileMode.Create))
				{
					CompressFileLZMA(inFile2, outFile2);
				}
			}
		}

		public static void CompressFileLZMA(Stream inFile, Stream outFile)
		{
			Encoder encoder = new Encoder();
			encoder.WriteCoderProperties(outFile);
			outFile.Write(BitConverter.GetBytes(inFile.Length), 0, 8);
			encoder.Code(inFile, outFile, inFile.Length, -1L, null);
			inFile.Position = 0L;
			outFile.Position = 0L;
		}

		public static void DecompressFileLZMA(string inFile, string outFile)
		{
			Decoder decoder = new Decoder();
			using (FileStream inFile2 = new FileStream(inFile, FileMode.Open))
			{
				using (FileStream outFile2 = new FileStream(outFile, FileMode.Create))
				{
					DecompressFileLZMA(inFile2, outFile2);
				}
			}
		}

		public static void DecompressFileLZMA(Stream inFile, Stream outFile)
		{
			Decoder decoder = new Decoder();
			byte[] array = new byte[5];
			inFile.Read(array, 0, 5);
			byte[] array2 = new byte[8];
			inFile.Read(array2, 0, 8);
			long outSize = BitConverter.ToInt64(array2, 0);
			decoder.SetDecoderProperties(array);
			decoder.Code(inFile, outFile, inFile.Length, outSize, null);
			inFile.Position = 0L;
			outFile.Position = 0L;
		}
	}
}
