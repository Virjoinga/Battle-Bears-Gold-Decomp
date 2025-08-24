using System;
using System.Collections.Generic;
using SkyVu.Common.Enums;
using SkyVuEngine.Core;
using SkyVuEngine.SkyVuNetwork.Client.Entities;

namespace SkyVuEngine.SkyVuNetwork.Client.Managers
{
	public class ItemsManager
	{
		public class GameItem
		{
			public class GameItemAttribute
			{
				public int GameItemAttributeId { get; set; }

				public int GameItemAttributeType { get; set; }

				public int GameItemId { get; set; }

				public string StringValue { get; set; }

				public double? Value { get; set; }

				public GameItemAttribute(GameItemAttributeEntity gameItemAttributeEntity)
				{
					GameItemAttributeId = gameItemAttributeEntity.GameItemAttributeId;
					GameItemAttributeType = gameItemAttributeEntity.GameItemAttributeType;
					GameItemId = gameItemAttributeEntity.GameItemId;
					StringValue = gameItemAttributeEntity.StringValue;
					Value = gameItemAttributeEntity.Value;
				}
			}

			public int GameItemId { get; set; }

			public int GameId { get; set; }

			public string Title { get; set; }

			public string Description { get; set; }

			public int GasCost { get; set; }

			public int JouleCost { get; set; }

			public string LabelName { get; set; }

			public decimal? SalesPercent { get; set; }

			public List<GameItemAttribute> Attributes { get; set; }

			public GameItem(GameItemEntity gameItemEntity)
			{
				GameItemId = gameItemEntity.GameItemId;
				GameId = gameItemEntity.GameId;
				Title = gameItemEntity.Title;
				Description = gameItemEntity.Description;
				GasCost = gameItemEntity.GasCost;
				JouleCost = gameItemEntity.JouleCost;
				LabelName = gameItemEntity.LabelName;
				SalesPercent = gameItemEntity.SalesPercent;
				Attributes = new List<GameItemAttribute>();
			}
		}

		private GameManager _gameManager = null;

		private ISkyVuNetworkProxy _proxy = null;

		private List<GameItem> _gameItems = null;

		private bool _isGameItemsFinished = false;

		public Action<List<GameItemEntity>> SuccessCallback { get; set; }

		public Action<string> FailureCallback { get; set; }

		public ItemsManager(ISkyVuNetworkProxy proxy, GameManager gameManager)
		{
			_gameManager = gameManager;
			_proxy = proxy;
			SuccessCallback = null;
			FailureCallback = null;
		}

		public bool GetGameItems(ref List<GameItem> gameItems)
		{
			gameItems = _gameItems;
			return _isGameItemsFinished;
		}

		public void PopulateGameItems()
		{
			_proxy.CallService(Services.GetGameItems, new GameItemEntity
			{
				GameId = (int)_gameManager.Game,
				Title = ".",
				Description = ".",
				UserId = UserManager.AnonymousUser.UserId,
				Catalog = UserManager.AnonymousUser.Catalog,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				PopulateGameItemAttributes(_proxy.GetEntities<GameItemEntity>(json));
			}, delegate
			{
				_isGameItemsFinished = true;
			});
		}

		private void PopulateGameItemAttributes(List<GameItemEntity> gameItemEntities)
		{
			_proxy.CallService(Services.GetGameItemAttributes, new GameItemEntity
			{
				GameId = (int)_gameManager.Game,
				Title = ".",
				Description = ".",
				UserId = UserManager.AnonymousUser.UserId,
				Catalog = UserManager.AnonymousUser.Catalog,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				CreateGameItemList(gameItemEntities, _proxy.GetEntities<GameItemAttributeEntity>(json));
			}, delegate
			{
				_isGameItemsFinished = true;
			});
		}

		public void CreateGameItemList(List<GameItemEntity> gameItemEntities, List<GameItemAttributeEntity> gameItemAttributeEntities)
		{
			if (gameItemEntities == null || gameItemAttributeEntities == null)
			{
				throw new NullReferenceException("SkyVuEngine.SkyVuNetwork.Client.Managers.ItemManager.CreateGameItemList: Game Items did not populate correctly.");
			}
			_gameItems = new List<GameItem>();
			foreach (GameItemEntity gameItemEntity in gameItemEntities)
			{
				GameItem gameItem = new GameItem(gameItemEntity);
				if (gameItemAttributeEntities != null)
				{
					foreach (GameItemAttributeEntity gameItemAttributeEntity in gameItemAttributeEntities)
					{
						if (gameItemAttributeEntity.GameItemId == gameItemEntity.GameItemId)
						{
							GameItem.GameItemAttribute item = new GameItem.GameItemAttribute(gameItemAttributeEntity);
							gameItem.Attributes.Add(item);
						}
					}
				}
				_gameItems.Add(gameItem);
			}
			_isGameItemsFinished = true;
		}

		public void GetUserItems(UserEntity userEntity, Action<List<UserGameItemEntity>> successCallback, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetUserGameItems, new UserGameItemEntity
			{
				GameId = (int)_gameManager.Game,
				GameItemId = 0,
				UserId = userEntity.UserId,
				Catalog = userEntity.Catalog,
				UserGameItemId = 0,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				if (successCallback != null)
				{
					successCallback(_proxy.GetEntities<UserGameItemEntity>(json));
				}
			}, delegate(string json)
			{
				if (failureCallback != null)
				{
					failureCallback(json);
				}
			});
		}

		public void UpdateUserItem(UserEntity userEntity, UserGameItemEntity gameItem, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.UpdateUserGameItems, new UserGameItemEntity
			{
				GameId = (int)_gameManager.Game,
				GameItemId = gameItem.GameItemId,
				Count = gameItem.Count,
				UserGameItemId = gameItem.UserGameItemId,
				UserId = userEntity.UserId,
				Catalog = userEntity.Catalog,
				MethodType = 2,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				if (successCallback != null)
				{
					successCallback(json);
				}
			}, delegate(string json)
			{
				if (failureCallback != null)
				{
					failureCallback(json);
				}
			});
		}

		public void CreateUserItems(UserEntity userEntity, GameItem gameItem, int count = 0, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.CreateUserGameItems, new UserGameItemEntity
			{
				GameId = (int)_gameManager.Game,
				GameItemId = gameItem.GameItemId,
				UserGameItemId = 0,
				Count = count,
				UserId = userEntity.UserId,
				Catalog = userEntity.Catalog,
				MethodType = 1,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				if (successCallback != null)
				{
					successCallback(json);
				}
			}, delegate(string json)
			{
				if (failureCallback != null)
				{
					failureCallback(json);
				}
			});
		}
	}
}
