using System.Collections.Generic;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class CompiledGameDataEntity : BaseEntity
	{
		private List<GameItemEntity> _gameItemsList;

		private List<GameItemAttributeEntity> _gameItemAttributeList;

		private List<NewsFeedEntity> _NewsFeedList;

		private List<GameSaleEntity> _GameSaleList;

		private List<GameSettingEntity> _GameSettingList;

		private List<BundleEntity> _BundleList;

		private List<BundleItemEntity> _BundleItemList;

		private List<GameCurrencyConversionEntity> _GameCurrencyConversionList;

		private List<IapPackageEntity> _IapPackageList;

		private List<PatchEntity> _PatchList;

		private List<AchievementEntity> _AchievementList;

		private List<LeaderBoardEntity> _LeaderboardsList;

		private List<string> _CustomGameDataList;

		public List<GameItemEntity> GameItems
		{
			get
			{
				return _gameItemsList;
			}
			set
			{
				_gameItemsList = value;
			}
		}

		public List<GameItemAttributeEntity> GameItemAttributes
		{
			get
			{
				return _gameItemAttributeList;
			}
			set
			{
				_gameItemAttributeList = value;
			}
		}

		public List<NewsFeedEntity> NewsFeeds
		{
			get
			{
				return _NewsFeedList;
			}
			set
			{
				_NewsFeedList = value;
			}
		}

		public List<GameSaleEntity> GameSales
		{
			get
			{
				return _GameSaleList;
			}
			set
			{
				_GameSaleList = value;
			}
		}

		public List<GameSettingEntity> GameSettings
		{
			get
			{
				return _GameSettingList;
			}
			set
			{
				_GameSettingList = value;
			}
		}

		public List<BundleEntity> Bundles
		{
			get
			{
				return _BundleList;
			}
			set
			{
				_BundleList = value;
			}
		}

		public List<BundleItemEntity> BundleItems
		{
			get
			{
				return _BundleItemList;
			}
			set
			{
				_BundleItemList = value;
			}
		}

		public List<GameCurrencyConversionEntity> GameCurrencyConversions
		{
			get
			{
				return _GameCurrencyConversionList;
			}
			set
			{
				_GameCurrencyConversionList = value;
			}
		}

		public List<IapPackageEntity> IapPackages
		{
			get
			{
				return _IapPackageList;
			}
			set
			{
				_IapPackageList = value;
			}
		}

		public List<PatchEntity> Patches
		{
			get
			{
				return _PatchList;
			}
			set
			{
				_PatchList = value;
			}
		}

		public List<AchievementEntity> Achievements
		{
			get
			{
				return _AchievementList;
			}
			set
			{
				_AchievementList = value;
			}
		}

		public List<LeaderBoardEntity> Leaderboards
		{
			get
			{
				return _LeaderboardsList;
			}
			set
			{
				_LeaderboardsList = value;
			}
		}

		public List<string> CustomGameData
		{
			get
			{
				return _CustomGameDataList;
			}
			set
			{
				_CustomGameDataList = value;
			}
		}

		public CompiledGameDataEntity()
		{
			_gameItemsList = new List<GameItemEntity>();
			_gameItemAttributeList = new List<GameItemAttributeEntity>();
			_NewsFeedList = new List<NewsFeedEntity>();
			_GameSaleList = new List<GameSaleEntity>();
			_GameSettingList = new List<GameSettingEntity>();
			_BundleList = new List<BundleEntity>();
			_BundleItemList = new List<BundleItemEntity>();
			_GameCurrencyConversionList = new List<GameCurrencyConversionEntity>();
			_IapPackageList = new List<IapPackageEntity>();
			_PatchList = new List<PatchEntity>();
			_AchievementList = new List<AchievementEntity>();
			_LeaderboardsList = new List<LeaderBoardEntity>();
			_CustomGameDataList = new List<string>();
			base.EntityName = "compiledgamedata";
		}

		public override string Serialize(bool baseData)
		{
			JsonWriter jsonWriter = new JsonWriter();
			jsonWriter.WriteObjectStart();
			SerializeBase(jsonWriter);
			Serialize(_gameItemsList, jsonWriter, "gi");
			Serialize(_gameItemAttributeList, jsonWriter, "gia");
			Serialize(_NewsFeedList, jsonWriter, "nf");
			Serialize(_GameSaleList, jsonWriter, "gs");
			Serialize(_GameSettingList, jsonWriter, "set");
			Serialize(_BundleList, jsonWriter, "bun");
			Serialize(_BundleItemList, jsonWriter, "bi");
			Serialize(_GameCurrencyConversionList, jsonWriter, "gcc");
			Serialize(_IapPackageList, jsonWriter, "iap");
			Serialize(_PatchList, jsonWriter, "pat");
			Serialize(_AchievementList, jsonWriter, "ach");
			Serialize(_LeaderboardsList, jsonWriter, "lb");
			Serialize(_CustomGameDataList, jsonWriter, "cgd");
			jsonWriter.WriteObjectEnd();
			return jsonWriter.ToString();
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
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							GameItemEntity gameItemEntity = new GameItemEntity();
							gameItemEntity.Populate(jsonReader, false);
							_gameItemsList.Add(gameItemEntity);
						}
						break;
					case "gia":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							GameItemAttributeEntity gameItemAttributeEntity = new GameItemAttributeEntity();
							gameItemAttributeEntity.Populate(jsonReader, false);
							_gameItemAttributeList.Add(gameItemAttributeEntity);
						}
						break;
					case "nf":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							NewsFeedEntity newsFeedEntity = new NewsFeedEntity();
							newsFeedEntity.Populate(jsonReader, false);
							_NewsFeedList.Add(newsFeedEntity);
						}
						break;
					case "gs":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							GameSaleEntity gameSaleEntity = new GameSaleEntity();
							gameSaleEntity.Populate(jsonReader, false);
							_GameSaleList.Add(gameSaleEntity);
						}
						break;
					case "set":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							GameSettingEntity gameSettingEntity = new GameSettingEntity();
							gameSettingEntity.Populate(jsonReader, false);
							_GameSettingList.Add(gameSettingEntity);
						}
						break;
					case "bun":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							BundleEntity bundleEntity = new BundleEntity();
							bundleEntity.Populate(jsonReader, false);
							_BundleList.Add(bundleEntity);
						}
						break;
					case "bi":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							BundleItemEntity bundleItemEntity = new BundleItemEntity();
							bundleItemEntity.Populate(jsonReader, false);
							_BundleItemList.Add(bundleItemEntity);
						}
						break;
					case "gcc":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							GameCurrencyConversionEntity gameCurrencyConversionEntity = new GameCurrencyConversionEntity();
							gameCurrencyConversionEntity.Populate(jsonReader, false);
							_GameCurrencyConversionList.Add(gameCurrencyConversionEntity);
						}
						break;
					case "iap":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							IapPackageEntity iapPackageEntity = new IapPackageEntity();
							iapPackageEntity.Populate(jsonReader, false);
							_IapPackageList.Add(iapPackageEntity);
						}
						break;
					case "pat":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							PatchEntity patchEntity = new PatchEntity();
							patchEntity.Populate(jsonReader, false);
							_PatchList.Add(patchEntity);
						}
						break;
					case "ach":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							AchievementEntity achievementEntity = new AchievementEntity();
							achievementEntity.Populate(jsonReader, false);
							_AchievementList.Add(achievementEntity);
						}
						break;
					case "lb":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							LeaderBoardEntity leaderBoardEntity = new LeaderBoardEntity();
							leaderBoardEntity.Populate(jsonReader, false);
							_LeaderboardsList.Add(leaderBoardEntity);
						}
						break;
					case "cgd":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							string item = (string)jsonReader.Value;
							_CustomGameDataList.Add(item);
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
