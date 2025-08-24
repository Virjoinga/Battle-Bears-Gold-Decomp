using System;
using System.Collections.Generic;
using SkyVu.Common;
using SkyVu.Common.Enums;
using SkyVuEngine.Core;
using SkyVuEngine.SkyVuNetwork.Client.Entities;
using UnityEngine;

namespace SkyVuEngine.SkyVuNetwork.Client.Managers
{
	public class UserManager
	{
		private const string CATALOG_DELIMITER = "::";

		private GameManager _gameManager = null;

		private ISkyVuNetworkProxy _proxy = null;

		private const string DEFAULT_CATALOG = "b741979050fb";

		public UserEntity CurrentUser { get; set; }

		public Action<UserEntity> SuccessCallback { get; set; }

		public Action<string> FailureCallback { get; set; }

		public static UserEntity AnonymousUser
		{
			get
			{
				UserEntity userEntity = new UserEntity();
				userEntity.GamerTag = "anonymous";
				userEntity.UserId = 1;
				userEntity.Catalog = "b741979050fb";
				return userEntity;
			}
		}

		public UserManager(ISkyVuNetworkProxy proxy, GameManager gameManager)
		{
			_gameManager = gameManager;
			_proxy = proxy;
			CurrentUser = AnonymousUser;
			SuccessCallback = null;
			FailureCallback = null;
		}

		public void LoginUser_SkyVu(int userId, string catalog, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetCategory, new CategoryEntity
			{
				UserId = userId,
				Catalog = catalog,
				GameId = (int)_gameManager.Game,
				AccountType = 100,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				MethodType = 4
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

		public void LoginUser_OAuth(int userId, string accountToken, AccountTypes accountType, string aux1 = null, string aux2 = null, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetCategory, new CategoryEntity
			{
				UserId = userId,
				GameId = (int)_gameManager.Game,
				AccountType = (int)accountType,
				AccountToken = accountToken,
				Aux1 = aux1,
				Aux2 = aux2,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				MethodType = 4
			}, delegate(string json)
			{
				CategoryEntity categoryEntity = new CategoryEntity();
				if (categoryEntity.Populate(json))
				{
					LoadUser(categoryEntity, successCallback, failureCallback);
				}
				else
				{
					Debug.Log("UserManager: LoginUser_OAuth, Category did not populate");
				}
			}, delegate(string json)
			{
				if (failureCallback != null)
				{
					failureCallback(json);
				}
			});
		}

		public void LoginUser_OAuth(string email, string accountToken, AccountTypes accountType, string aux1 = null, string aux2 = null, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetCategory, new CategoryEntity
			{
				UserId = 0,
				Email = email,
				GameId = (int)_gameManager.Game,
				AccountType = (int)accountType,
				AccountToken = accountToken,
				Aux1 = aux1,
				Aux2 = aux2,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				MethodType = 4
			}, delegate(string json)
			{
				CategoryEntity categoryEntity = new CategoryEntity();
				if (categoryEntity.Populate(json))
				{
					LoadUser(categoryEntity, successCallback, failureCallback);
				}
				else
				{
					Debug.Log("UserManager: LoginUser_OAuth, Category did not populate");
				}
			}, delegate(string json)
			{
				if (failureCallback != null)
				{
					failureCallback(json);
				}
			});
		}

		public void LoadUser(int userId)
		{
			_proxy.CallService(Services.GetUserById, new UserEntity
			{
				UserId = userId,
				GamerTag = ".",
				Catalog = "b741979050fb",
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				CurrentUser = _proxy.GetEntity<UserEntity>(json);
				if (SuccessCallback != null)
				{
					SuccessCallback(CurrentUser);
				}
			}, FailureCallback);
		}

		public void LoadUser(string gamerTag)
		{
			_proxy.CallService(Services.GetUserById, new UserEntity
			{
				UserId = 0,
				GamerTag = gamerTag,
				Catalog = "b741979050fb",
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				CurrentUser = _proxy.GetEntity<UserEntity>(json);
				if (SuccessCallback != null)
				{
					SuccessCallback(CurrentUser);
				}
			}, FailureCallback);
		}

		public void LoadUsers(int userId, Action<string> successCallback, Action<string> failureCallback)
		{
			_proxy.CallService(Services.GetUserById, new UserEntity
			{
				UserId = userId,
				GamerTag = ".",
				Catalog = "b741979050fb",
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

		public void LoadUser(CategoryEntity categoryEntity, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetUserById, new UserEntity
			{
				UserId = categoryEntity.UserId,
				GamerTag = ".",
				Version = categoryEntity.Version,
				Catalog = categoryEntity.Catalog,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string userJson)
			{
				CurrentUser = _proxy.GetEntity<UserEntity>(userJson);
				if (CurrentUser != null)
				{
					CurrentUser.Catalog = categoryEntity.Catalog;
				}
				if (successCallback != null)
				{
					successCallback(userJson);
				}
			}, delegate(string userJson)
			{
				if (failureCallback != null)
				{
					failureCallback(userJson);
				}
			});
		}

		public void GetBatchedUserData(Action<BatchedUserDataEntity> success = null, Action<string> failure = null)
		{
			_proxy.CallService(Services.GetBatchedUserData, new BatchedUserDataEntity
			{
				Version = _gameManager.GameVersion,
				DeviceSerial = _gameManager.DeviceSerial,
				UserId = CurrentUser.UserId,
				Catalog = CurrentUser.Catalog,
				MethodType = 4,
				GameId = (int)_gameManager.Game
			}, delegate(string json)
			{
				BatchedUserDataEntity entity = _proxy.GetEntity<BatchedUserDataEntity>(json);
				if (entity != null && success != null)
				{
					success(entity);
				}
			}, delegate(string message)
			{
				if (failure != null)
				{
					failure(message);
				}
			});
		}

		public void CreateUser(string gamerTag, string category, Action<string> successCallback, Action<int> failureCallback)
		{
			_proxy.CallService(Services.CreateNewUser, new UserEntity
			{
				GamerTag = gamerTag,
				MethodType = 1,
				Catalog = category,
				DeviceSerial = _gameManager.DeviceSerial,
				UserId = AnonymousUser.UserId
			}, delegate(string json)
			{
				Debug.Log("Create User: " + json);
				int? num2 = Parsers.ParseInt(json);
				CurrentUser.UserId = num2.Value;
				CurrentUser.Catalog = category;
				CurrentUser.GamerTag = gamerTag;
				if (successCallback != null && num2.HasValue)
				{
					successCallback(json);
				}
			}, delegate(string json)
			{
				int? num = Parsers.ParseInt(json);
				if (failureCallback != null)
				{
					if (!num.HasValue)
					{
						failureCallback(-999);
					}
					else
					{
						failureCallback(num.Value);
					}
				}
			});
		}

		public void CreateAccount(string email, AccountTypes accountType, string accountToken, Action<int> successCallback, Action<int> failureCallback, string aux1 = null, string aux2 = null)
		{
			CreateAccount(email, accountType, accountToken, CurrentUser.Catalog, successCallback, failureCallback);
		}

		public void CreateAccount(string email, AccountTypes accountType, string accountToken, string catalog, Action<int> successCallback, Action<int> failureCallback, string aux1 = null, string aux2 = null)
		{
			_proxy.CallService(Services.CreateCategory, new CategoryEntity
			{
				MethodType = 1,
				UserId = CurrentUser.UserId,
				Catalog = catalog,
				AccountToken = accountToken,
				Aux1 = aux1,
				Aux2 = aux2,
				Email = email,
				AccountType = (int)accountType,
				GameId = (int)_gameManager.Game,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				Debug.Log("CreateAccount = " + json);
				CategoryEntity entity = _proxy.GetEntity<CategoryEntity>(json);
				if (successCallback != null && entity != null)
				{
					successCallback(entity.UserId);
				}
			}, delegate(string json)
			{
				Debug.Log("CreateAccount FAILED = " + json);
				int? num = Parsers.ParseInt(json);
				if (failureCallback != null && num.HasValue)
				{
					failureCallback(num.Value);
				}
			});
		}

		public void GetBuddyRequests(Action<List<BuddyRequestEntity>> successCallback, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetBuddyRequestById, CurrentUser, delegate(string json)
			{
				List<BuddyRequestEntity> entities = _proxy.GetEntities<BuddyRequestEntity>(json);
				if (successCallback != null)
				{
					successCallback(entities);
				}
			}, delegate(string json)
			{
				if (failureCallback != null)
				{
					failureCallback(json);
				}
				else if (FailureCallback != null)
				{
					FailureCallback(json);
				}
			});
		}

		public void GetBuddyList(Action<List<BuddyListEntity>> successCallback, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetBuddyListById, CurrentUser, delegate(string json)
			{
				List<BuddyListEntity> entities = _proxy.GetEntities<BuddyListEntity>(json);
				if (successCallback != null)
				{
					successCallback(entities);
				}
			}, delegate(string json)
			{
				if (failureCallback != null)
				{
					failureCallback(json);
				}
				else if (FailureCallback != null)
				{
					FailureCallback(json);
				}
			});
		}

		[Obsolete("No longer need to pass in the gameId", true)]
		public void CreateBuddy(int requestedUser, int gameId, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.CreateBuddyRequest, new BuddyRequestEntity
			{
				GameId = gameId,
				RequestedUserId = requestedUser,
				RequesterUserId = CurrentUser.UserId,
				BuddyRequestStatusId = 1,
				UserId = CurrentUser.UserId,
				Catalog = CurrentUser.Catalog,
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

		public void CreateBuddy(int requestedUserId, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.CreateBuddyRequest, new BuddyRequestEntity
			{
				GameId = (int)_gameManager.Game,
				RequestedUserId = requestedUserId,
				RequesterUserId = CurrentUser.UserId,
				BuddyRequestStatusId = 1,
				UserId = CurrentUser.UserId,
				Catalog = CurrentUser.Catalog,
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

		public void DeleteBuddyList(BuddyListEntity buddyList, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			buddyList.Version = _gameManager.GameVersion;
			buddyList.DeviceSerial = _gameManager.DeviceSerial;
			buddyList.Catalog = CurrentUser.Catalog;
			buddyList.UserId = CurrentUser.UserId;
			buddyList.MethodType = 3;
			_proxy.CallService(Services.DeleteBuddyList, buddyList, successCallback, failureCallback);
		}

		public void UpdateBuddyRequest(int requestId, int requestedUser, BuddyRequestStatus requestStatus, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.UpdateBuddyRequestStat, new BuddyRequestEntity
			{
				RequestedUserId = requestedUser,
				RequesterUserId = CurrentUser.UserId,
				BuddyRequestId = requestId,
				BuddyRequestStatusId = (int)requestStatus,
				MethodType = 2,
				Catalog = CurrentUser.Catalog,
				UserId = CurrentUser.UserId,
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

		public void CreateUserItem(int gameItemId, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.CreateUserGameItems, new UserGameItemEntity
			{
				GameItemId = gameItemId,
				GameId = (int)_gameManager.Game,
				UserId = CurrentUser.UserId,
				Catalog = CurrentUser.Catalog,
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

		public void GetUserItems(Action<string> successCallback, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetUserGameItems, new UserGameItemEntity
			{
				GameItemId = 0,
				GameId = (int)_gameManager.Game,
				UserId = CurrentUser.UserId,
				Catalog = CurrentUser.Catalog,
				MethodType = 4,
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

		public void CreateUserCustomData(string customData, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.CreateUserCustomData, new UserCustomDataEntity
			{
				GameId = (int)_gameManager.Game,
				UserId = CurrentUser.UserId,
				Catalog = CurrentUser.Catalog,
				UserCustomDataId = CurrentUser.UserId,
				Data = customData,
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

		public void GetUserCustomData(Action<string> successCallback, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetUserCustomData, new UserCustomDataEntity
			{
				GameId = (int)_gameManager.Game,
				Data = "",
				UserId = CurrentUser.UserId,
				Catalog = CurrentUser.Catalog,
				UserCustomDataId = CurrentUser.UserId,
				MethodType = 4,
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

		public void GetUserEconomy(Action<string> successCallback, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetUserE, new UserEconomyEntity
			{
				Gas = 0,
				Joules = 0,
				GameId = (int)_gameManager.Game,
				UserId = CurrentUser.UserId,
				Catalog = CurrentUser.Catalog,
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

		public void CreateUserEconomy(int gas, int joules, Action<string> successCallback, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.CreateUserE, new UserEconomyEntity
			{
				Gas = gas,
				Joules = joules,
				GameId = (int)_gameManager.Game,
				UserId = CurrentUser.UserId,
				Catalog = CurrentUser.Catalog,
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

		public void UpdateUserEconomy(int gas, int joules, Action<string> successCallback, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.UpdateUserE, new UserEconomyEntity
			{
				Gas = gas,
				Joules = joules,
				GameId = (int)_gameManager.Game,
				UserId = CurrentUser.UserId,
				Catalog = CurrentUser.Catalog,
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

		public void VerifyIap(string token, string token2, GamePlatformTypes gamePlatformType, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.IapVerify, new IapVerifyEntity
			{
				MethodType = 4,
				Catalog = CurrentUser.Catalog,
				UserId = CurrentUser.UserId,
				GamePlatformType = (int)gamePlatformType,
				Token1 = token,
				Token2 = token2,
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

		public void GetUser(int userId, ISkyVuNetworkProxy proxy, Action<UserEntity> SuccessCallBack, Action<string> FailureCallback)
		{
			proxy.CallService(Services.GetUserById, new UserEntity
			{
				UserId = userId,
				GamerTag = ".",
				Catalog = "b741979050fb",
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				SuccessCallBack(proxy.GetEntity<UserEntity>(json));
			}, FailureCallback);
		}

		public void GetUser(string gamerTag, ISkyVuNetworkProxy proxy, Action<UserEntity> SuccessCallBack, Action<string> FailureCallback)
		{
			proxy.CallService(Services.GetUserById, new UserEntity
			{
				UserId = 0,
				GamerTag = gamerTag,
				Catalog = "b741979050fb",
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				SuccessCallBack(proxy.GetEntity<UserEntity>(json));
			}, FailureCallback);
		}

		public void GetUser(int userId, Action<UserEntity> SuccessCallBack, Action<string> FailureCallback)
		{
			_proxy.CallService(Services.GetUserById, new UserEntity
			{
				UserId = userId,
				GamerTag = ".",
				Catalog = "b741979050fb",
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				SuccessCallBack(_proxy.GetEntity<UserEntity>(json));
			}, FailureCallback);
		}

		public void GetUser(string gamerTag, Action<UserEntity> SuccessCallBack, Action<string> FailureCallback)
		{
			_proxy.CallService(Services.GetUserById, new UserEntity
			{
				UserId = 0,
				GamerTag = gamerTag,
				Catalog = "b741979050fb",
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				SuccessCallBack(_proxy.GetEntity<UserEntity>(json));
			}, FailureCallback);
		}

		public void GetUserIdByCategory(AccountTypes type, Action<int?> successCallback, Action<string> failureCallback = null, string email = null, string aux1 = null, string aux2 = null)
		{
			_proxy.CallService(Services.GetUserByCategory, new CategoryEntity
			{
				Email = email,
				DeviceSerial = _gameManager.DeviceSerial,
				GameId = (int)_gameManager.Game,
				AccountType = (int)type,
				Version = _gameManager.GameVersion,
				Aux1 = aux1,
				Aux2 = aux2
			}, delegate(string json)
			{
				if (json == "null" || string.IsNullOrEmpty(json))
				{
					if (successCallback != null)
					{
						successCallback(null);
					}
				}
				else
				{
					GeneralMessageEntity generalMessageEntity = new GeneralMessageEntity();
					if (generalMessageEntity.Populate(json))
					{
						string[] array = generalMessageEntity.Message.Split(':');
						if (array.Length > 0)
						{
							int? num = Parsers.ParseInt(array[0]);
							if (num.HasValue)
							{
								if (successCallback != null)
								{
									successCallback(num.Value);
								}
								return;
							}
						}
					}
					successCallback(null);
				}
			}, delegate(string json)
			{
				if (failureCallback != null)
				{
					failureCallback(json);
				}
			});
		}

		public void UpdateEmail(string email, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.UpdateAccount, new CategoryEntity
			{
				MethodType = 2,
				GamerTag = CurrentUser.GamerTag,
				UserId = CurrentUser.UserId,
				AccountToken = CurrentUser.Catalog,
				AccountType = 100,
				GameId = (int)_gameManager.Game,
				DeviceSerial = _gameManager.DeviceSerial,
				Email = email,
				Version = _gameManager.GameVersion,
				Catalog = CurrentUser.Catalog
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

		public void UpdateGamerTag(string gamerTag, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.UpdateUser, new UserEntity
			{
				MethodType = 2,
				Catalog = CurrentUser.Catalog,
				UserId = CurrentUser.UserId,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				GamerTag = gamerTag
			}, delegate(string json)
			{
				CurrentUser.GamerTag = gamerTag;
				if (successCallback != null)
				{
					successCallback(json);
				}
			}, delegate(string message)
			{
				if (failureCallback != null)
				{
					failureCallback(message);
				}
			});
		}

		public void UpdateCatalog(string catalog, Action<string> successsCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.UpdateUser, new UserEntity
			{
				MethodType = 2,
				Catalog = CurrentUser.Catalog + "::" + catalog,
				UserId = CurrentUser.UserId,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				GamerTag = CurrentUser.GamerTag
			}, delegate(string message)
			{
				if (successsCallback != null)
				{
					successsCallback(message);
				}
			}, delegate(string message)
			{
				if (failureCallback != null)
				{
					failureCallback(message);
				}
			});
		}

		public void UpdateUser(string catalog, string gamerTag, Action<string> successsCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.UpdateUser, new UserEntity
			{
				MethodType = 2,
				Catalog = CurrentUser.Catalog + "::" + catalog,
				UserId = CurrentUser.UserId,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				GamerTag = gamerTag
			}, delegate(string message)
			{
				if (successsCallback != null)
				{
					successsCallback(message);
				}
			}, delegate(string message)
			{
				if (failureCallback != null)
				{
					failureCallback(message);
				}
			});
		}

		[Obsolete("This is no longer being used in BBU (possibly used in bbf)", true)]
		public static void GetUserIdByAccount(CategoryEntity categoryEntity, AccountTypes accountType, ISkyVuNetworkProxy proxy, Action<List<int>> SuccessCallBack, Action<string> FailureCallback)
		{
			categoryEntity.UserId = 0;
			categoryEntity.Catalog = "b741979050fb";
			categoryEntity.AccountType = (int)accountType;
			if (categoryEntity.GamerTag == null || categoryEntity.GamerTag == "")
			{
				categoryEntity.GamerTag = ".";
			}
			if (categoryEntity.Email == null || categoryEntity.Email == "")
			{
				categoryEntity.Email = ".";
			}
			proxy.CallService(Services.GetUserByCategory, categoryEntity, delegate(string json)
			{
				GeneralMessageEntity generalMessageEntity = new GeneralMessageEntity();
				if (generalMessageEntity.Populate(json))
				{
					string[] array = generalMessageEntity.Message.Split(':');
					if (array == null)
					{
						FailureCallback(json);
					}
					List<int> list = new List<int>();
					string[] array2 = array;
					foreach (string s in array2)
					{
						int result;
						if (int.TryParse(s, out result))
						{
							list.Add(result);
						}
					}
					SuccessCallBack(list);
				}
				else
				{
					FailureCallback(json);
				}
			}, FailureCallback);
		}
	}
}
