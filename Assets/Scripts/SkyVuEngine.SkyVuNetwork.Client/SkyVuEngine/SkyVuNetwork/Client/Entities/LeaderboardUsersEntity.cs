using System.Collections.Generic;
using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class LeaderboardUsersEntity : BaseEntity
	{
		private string _users = string.Empty;

		private string _scores = string.Empty;

		public int LeaderBoardId { get; set; }

		public List<int> Users { get; set; }

		public List<int> Scores { get; set; }

		public LeaderboardUsersEntity()
		{
			Users = new List<int>();
			Scores = new List<int>();
			base.EntityName = "leaderboardusers";
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
			jsonWriter.WritePropertyName("i");
			jsonWriter.Write(LeaderBoardId);
			Users.ForEach(delegate(int b)
			{
				_users = _users + b + ";";
			});
			jsonWriter.WritePropertyName("users");
			jsonWriter.Write(_users);
			Scores.ForEach(delegate(int b)
			{
				_scores = _scores + b + ";";
			});
			jsonWriter.WritePropertyName("scores");
			jsonWriter.Write(_scores);
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
					if (reader.Value == null)
					{
						continue;
					}
					switch (reader.Value.ToString())
					{
					case "i":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							LeaderBoardId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'leaderBoardId' value in 'LeaderboardUsersEntity'");
					}
					case "users":
						reader.Read();
						_users = (string)reader.Value;
						break;
					case "scores":
						reader.Read();
						_scores = (string)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				string[] array = _users.Split(';');
				foreach (string toParse in array)
				{
					int? num2 = Parsers.ParseInt(toParse);
					if (num2.HasValue)
					{
						Users.Add(num2.Value);
					}
				}
				string[] array2 = _scores.Split(';');
				foreach (string toParse2 in array2)
				{
					int? num3 = Parsers.ParseInt(toParse2);
					if (num3.HasValue)
					{
						Scores.Add(num3.Value);
					}
				}
				return base.IsPopulated && LeaderBoardId > 0 && Users.Count > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
