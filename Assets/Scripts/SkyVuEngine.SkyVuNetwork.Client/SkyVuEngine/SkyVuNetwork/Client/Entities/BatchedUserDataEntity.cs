using System.Collections.Generic;
using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class BatchedUserDataEntity : BaseEntity
	{
		private UserEconomyEntity _userEconomy;

		private UserCustomDataEntity _userCustomData;

		private List<BuddyRequestEntity> _buddyRequests;

		private List<UserAchievementEntity> _userAchievements;

		private List<BuddyListEntity> _buddyLists;

		private List<UserLeaderBoardEntity> _userLeaderboards;

		private List<UserGameItemEntity> _userGameItems;

		public int GameId { get; set; }

		public List<BuddyRequestEntity> BuddyRequests
		{
			get
			{
				return _buddyRequests;
			}
			set
			{
				_buddyRequests = value;
			}
		}

		public UserEconomyEntity UserEconomy
		{
			get
			{
				return _userEconomy;
			}
			set
			{
				_userEconomy = value;
			}
		}

		public UserCustomDataEntity UserCustomData
		{
			get
			{
				return _userCustomData;
			}
			set
			{
				_userCustomData = value;
			}
		}

		public List<UserAchievementEntity> UserAchievements
		{
			get
			{
				return _userAchievements;
			}
			set
			{
				_userAchievements = value;
			}
		}

		public List<BuddyListEntity> BuddyLists
		{
			get
			{
				return _buddyLists;
			}
			set
			{
				_buddyLists = value;
			}
		}

		public List<UserLeaderBoardEntity> UserLeaderboards
		{
			get
			{
				return _userLeaderboards;
			}
			set
			{
				_userLeaderboards = value;
			}
		}

		public List<UserGameItemEntity> UserGameItems
		{
			get
			{
				return _userGameItems;
			}
			set
			{
				_userGameItems = value;
			}
		}

		public BatchedUserDataEntity()
		{
			_buddyRequests = new List<BuddyRequestEntity>();
			_userAchievements = new List<UserAchievementEntity>();
			_buddyLists = new List<BuddyListEntity>();
			_userLeaderboards = new List<UserLeaderBoardEntity>();
			_userGameItems = new List<UserGameItemEntity>();
			base.EntityName = "batcheduserdata";
		}

		public override string Serialize()
		{
			return Serialize(false);
		}

		public override string Serialize(bool baseData)
		{
			JsonWriter jsonWriter = new JsonWriter();
			jsonWriter.WriteObjectStart();
			SerializeBase(jsonWriter);
			jsonWriter.WritePropertyName("gi");
			jsonWriter.Write(GameId);
			if (_userEconomy != null)
			{
				jsonWriter.WritePropertyName("e");
				jsonWriter.Write(_userEconomy.Serialize());
			}
			if (_userCustomData != null)
			{
				jsonWriter.WritePropertyName("d");
				jsonWriter.Write(_userCustomData.Serialize());
			}
			Serialize(_buddyRequests, jsonWriter, "br");
			Serialize(_userAchievements, jsonWriter, "a");
			Serialize(_buddyLists, jsonWriter, "bl");
			Serialize(_userLeaderboards, jsonWriter, "l");
			Serialize(_userGameItems, jsonWriter, "i");
			jsonWriter.WriteObjectEnd();
			return jsonWriter.ToString();
		}

		public override bool Populate(JsonReader reader)
		{
			return base.Populate(reader, false);
		}

		public override bool Populate(string json, bool baseData)
		{
			try
			{
				if (json == null)
				{
					return false;
				}
				json = StripUnusedCharacters(json);
				JsonReader jsonReader = new JsonReader(json);
				jsonReader.Read();
				if (jsonReader.Token != JsonToken.ObjectStart)
				{
					return false;
				}
				while (jsonReader.Read() && jsonReader.Token != JsonToken.ObjectEnd)
				{
					if (jsonReader.Value == null)
					{
						continue;
					}
					switch (jsonReader.Value.ToString())
					{
					case "gi":
					{
						jsonReader.Read();
						int? num = Parsers.ParseInt(jsonReader.Value);
						if (num.HasValue)
						{
							GameId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'batchedUserDataEntity'");
					}
					case "e":
						_userEconomy = new UserEconomyEntity();
						_userEconomy.Populate(jsonReader, false);
						break;
					case "d":
						_userCustomData = new UserCustomDataEntity();
						_userCustomData.Populate(jsonReader, false);
						break;
					case "br":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							BuddyRequestEntity buddyRequestEntity = new BuddyRequestEntity();
							buddyRequestEntity.Populate(jsonReader, false);
							_buddyRequests.Add(buddyRequestEntity);
						}
						break;
					case "a":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							UserAchievementEntity userAchievementEntity = new UserAchievementEntity();
							userAchievementEntity.Populate(jsonReader, false);
							_userAchievements.Add(userAchievementEntity);
						}
						break;
					case "bl":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							BuddyListEntity buddyListEntity = new BuddyListEntity();
							buddyListEntity.Populate(jsonReader, false);
							_buddyLists.Add(buddyListEntity);
						}
						break;
					case "l":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							UserLeaderBoardEntity userLeaderBoardEntity = new UserLeaderBoardEntity();
							userLeaderBoardEntity.Populate(jsonReader, false);
							_userLeaderboards.Add(userLeaderBoardEntity);
						}
						break;
					case "i":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							UserGameItemEntity userGameItemEntity = new UserGameItemEntity();
							userGameItemEntity.Populate(jsonReader, false);
							_userGameItems.Add(userGameItemEntity);
						}
						break;
					default:
						PopulateBase(jsonReader);
						break;
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
