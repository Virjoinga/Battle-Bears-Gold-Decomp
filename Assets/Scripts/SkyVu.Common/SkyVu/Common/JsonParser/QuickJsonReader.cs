using System;
using System.Collections;
using System.IO;

namespace SkyVu.Common.JsonParser
{
	public class QuickJsonReader
	{
		public static object ReadJsonFile(string file)
		{
			string text = null;
			try
			{
				using (StreamReader streamReader = new StreamReader(file))
				{
					text = streamReader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			if (text == null)
			{
				return null;
			}
			return ReadJsonString(text);
		}

		public static object ReadJsonString(string json)
		{
			JsonReader jsonReader = new JsonReader(json);
			object result = ReadNext(jsonReader);
			jsonReader.Close();
			return result;
		}

		private static object ReadNext(JsonReader reader)
		{
			reader.Read();
			switch (reader.Token)
			{
			case JsonToken.ObjectStart:
				return ReadHashTable(reader);
			case JsonToken.ArrayStart:
				return ReadArray(reader);
			default:
				return null;
			}
		}

		private static Hashtable ReadHashTable(JsonReader reader)
		{
			Hashtable hashtable = new Hashtable();
			reader.Read();
			while (reader.Token != JsonToken.ObjectEnd)
			{
				if (reader.Token != JsonToken.PropertyName)
				{
					return null;
				}
				string key = reader.Value as string;
				object value = ReadValue(reader, true);
				hashtable.Add(key, value);
				reader.Read();
			}
			return hashtable;
		}

		private static ArrayList ReadArray(JsonReader reader)
		{
			ArrayList arrayList = new ArrayList();
			reader.Read();
			while (reader.Token != JsonToken.ArrayEnd)
			{
				arrayList.Add(ReadValue(reader, false));
				reader.Read();
			}
			return arrayList;
		}

		private static object ReadValue(JsonReader reader, bool readNew)
		{
			if (readNew)
			{
				reader.Read();
			}
			switch (reader.Token)
			{
			case JsonToken.ObjectStart:
				return ReadHashTable(reader);
			case JsonToken.ArrayStart:
				return ReadArray(reader);
			case JsonToken.Boolean:
			{
				bool flag = (bool)reader.Value;
				return flag;
			}
			case JsonToken.Int:
			{
				int num3 = (int)reader.Value;
				return num3;
			}
			case JsonToken.Long:
			{
				long num2 = (long)reader.Value;
				return num2;
			}
			case JsonToken.Double:
			{
				double num = (double)reader.Value;
				return num;
			}
			case JsonToken.String:
				return reader.Value;
			default:
				return new string[2] { "a", "b" };
			}
		}
	}
}
