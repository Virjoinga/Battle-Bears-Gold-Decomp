using System.Collections.Generic;
using System.Linq;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class BuddyListBatchEntity : BaseEntity
	{
		private string _buddies = string.Empty;

		public List<int> Buddies { get; set; }

		public BuddyListBatchEntity()
		{
			Buddies = new List<int>();
			base.EntityName = "buddylistbatch";
		}

		public override string Serialize()
		{
			return Serialize(true);
		}

		public override string Serialize(bool BaseData)
		{
			JsonWriter jsonWriter = new JsonWriter();
			jsonWriter.WriteObjectStart();
			if (BaseData)
			{
				SerializeBase(jsonWriter);
			}
			Buddies.ForEach(delegate(int b)
			{
				_buddies = _buddies + b + ";";
			});
			jsonWriter.WritePropertyName("bd");
			jsonWriter.Write(_buddies);
			jsonWriter.WriteObjectEnd();
			return jsonWriter.ToString();
		}

		public override bool Populate(JsonReader reader)
		{
			return Populate(reader, true);
		}

		public override bool Populate(JsonReader reader, bool BaseData)
		{
			try
			{
				while (reader.Read() && reader.Token != JsonToken.ObjectEnd)
				{
					if (reader.Value != null)
					{
						string text = reader.Value.ToString();
						if (text != null && text == "bd")
						{
							reader.Read();
							_buddies = (string)reader.Value;
						}
						else if (BaseData)
						{
							PopulateBase(reader);
						}
					}
				}
				_buddies.Split(';').ToList().ForEach(delegate(string b)
				{
					Buddies.Add(int.Parse(b));
				});
				return base.IsPopulated;
			}
			catch
			{
				return false;
			}
		}
	}
}
