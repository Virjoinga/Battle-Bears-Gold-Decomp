using System;
using System.Collections.Generic;
using System.IO;
using SkyVu.Common;
using SkyVu.Common.JsonParser;
using SkyVuEngine.Core.Crypto;

namespace SkyVuEngine.Core.Data
{
	public class LocalPersistantData
	{
		public int UserId { get; set; }

		public string GamerTag { get; set; }

		public string Category { get; set; }

		public byte[] Avatar { get; set; }

		public string AvatarFileName { get; set; }

		public List<ILocalGameDataComponent> DataComponents { get; set; }

		public LocalPersistantData()
		{
			UserId = 0;
			GamerTag = string.Empty;
			Category = string.Empty;
			AvatarFileName = string.Empty;
			Avatar = null;
			DataComponents = new List<ILocalGameDataComponent>();
		}

		public bool LoadData(string fileName, DataType dataType = DataType.Json)
		{
			if (!File.Exists(fileName))
			{
				throw new IOException("File not found.");
			}
			switch (dataType)
			{
			case DataType.Xml:
				throw new NotImplementedException("Xml is not supported.");
			case DataType.Json:
				return PopulateJson(fileName);
			default:
				return false;
			}
		}

		public bool SaveData(string fileName, DataType dataType = DataType.Json)
		{
			if (fileName.Length > 0)
			{
				switch (dataType)
				{
				case DataType.Xml:
					throw new NotImplementedException("Xml is not supported.");
				case DataType.Json:
					return SaveJson(fileName);
				}
			}
			return false;
		}

		private string ReadFile(string fileName)
		{
			try
			{
				using (StreamReader streamReader = new StreamReader(fileName))
				{
					return streamReader.ReadToEnd();
				}
			}
			catch
			{
				return null;
			}
		}

		private byte[] ReadImage(string fileName)
		{
			try
			{
				return File.ReadAllBytes(fileName);
			}
			catch
			{
				return null;
			}
		}

		private bool PopulateJson(string fileName)
		{
			StandardEncryption standardEncryption = new StandardEncryption();
			string text = ReadFile(fileName);
			if (text == null || text.Length == 0)
			{
				return false;
			}
			JsonReader reader = new JsonReader(standardEncryption.Decrypt(text));
			reader.Read();
			if (reader.Token != JsonToken.ObjectStart)
			{
				return false;
			}
			while (reader.Read() && reader.Token != JsonToken.ObjectEnd)
			{
				switch (((string)reader.Value).ToLower())
				{
				case "userid":
				{
					reader.Read();
					int? num = Parsers.ParseInt(reader.Value);
					if (num.HasValue)
					{
						UserId = num.Value;
						break;
					}
					throw new JsonException("Invalid 'userId' value.");
				}
				case "gamertag":
					reader.Read();
					GamerTag = (string)reader.Value;
					break;
				case "category":
					reader.Read();
					Category = (string)reader.Value;
					break;
				case "avatar":
					reader.Read();
					AvatarFileName = (string)reader.Value;
					Avatar = ReadImage(AvatarFileName);
					break;
				default:
					DataComponents.ForEach(delegate(ILocalGameDataComponent c)
					{
						c.LoadData(reader.Value.ToString(), reader, DataType.Json);
					});
					break;
				}
			}
			return true;
		}

		private bool SaveJson(string fileName)
		{
			StandardEncryption standardEncryption = new StandardEncryption();
			JsonWriter writer = new JsonWriter();
			writer.WriteObjectStart();
			DataComponents.ForEach(delegate(ILocalGameDataComponent c)
			{
				c.SaveData(writer, DataType.Json);
			});
			writer.WritePropertyName("userid");
			writer.Write(UserId);
			writer.WritePropertyName("gamertag");
			writer.Write(GamerTag);
			writer.WritePropertyName("category");
			writer.Write(Category);
			writer.WritePropertyName("avatar");
			writer.Write(AvatarFileName);
			writer.WriteObjectEnd();
			File.WriteAllText(fileName, standardEncryption.Encrypt(writer.ToString()));
			return true;
		}
	}
}
