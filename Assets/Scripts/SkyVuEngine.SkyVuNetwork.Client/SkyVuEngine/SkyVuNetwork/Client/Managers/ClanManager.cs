using System;
using System.Collections.Generic;
using SkyVu.Common;
using SkyVu.Common.Enums;
using SkyVuEngine.Core;
using SkyVuEngine.SkyVuNetwork.Client.Entities;
using UnityEngine;

namespace SkyVuEngine.SkyVuNetwork.Client.Managers
{
	public class ClanManager
	{
		private List<ClanEntity> _clans;

		private ClanEntity _userClan;

		private List<ClanEntity> _clanInvites;

		private ISkyVuNetworkProxy _proxy;

		private GameManager _gameManager;

		private List<int> _clanInfo;

		public List<ClanEntity> GameClans
		{
			get
			{
				return _clans;
			}
		}

		public ClanEntity UserClan
		{
			get
			{
				return _userClan;
			}
			set
			{
				_userClan = value;
			}
		}

		public List<ClanEntity> ClanInvites
		{
			get
			{
				return _clanInvites;
			}
		}

		public bool UserHasClan { get; private set; }

		public ClanManager(ISkyVuNetworkProxy proxy, GameManager gameManager)
		{
			_proxy = proxy;
			_gameManager = gameManager;
			_clans = new List<ClanEntity>();
			_clanInfo = new List<int>();
			_userClan = null;
		}

		public void GetClanById(int clanId, Action<ClanEntity> success, Action<string> failure)
		{
			GetClanById(clanId, false, success, failure);
		}

		public void GetClanById(int clanId, bool forceUpdate, Action<ClanEntity> success, Action<string> failure)
		{
			if (!forceUpdate && _clanInfo.Contains(clanId))
			{
				ClanEntity clanEntity = _clans.Find((ClanEntity clan) => clan.ClanId == clanId);
				if (clanEntity != null && success != null)
				{
					success(clanEntity);
					return;
				}
			}
			_proxy.CallService(Services.GetClanById, new ClanEntity
			{
				GameId = (int)_gameManager.Game,
				Version = _gameManager.GameVersion,
				DeviceSerial = _gameManager.DeviceSerial,
				ClanId = clanId,
				Catalog = UserManager.AnonymousUser.Catalog
			}, delegate(string json)
			{
				if ((string.IsNullOrEmpty(json) || json == "null") && success != null)
				{
					success(null);
				}
				ClanEntity entity = _proxy.GetEntity<ClanEntity>(json);
				_clans = _clans ?? new List<ClanEntity>();
				int num = _clans.FindIndex((ClanEntity clanSearch) => clanSearch.ClanId == clanId);
				if (num != -1)
				{
					_clanInfo.Add(clanId);
					_clans[num] = entity;
				}
				if (success != null)
				{
					success(entity);
				}
			}, failure);
		}

		public void GetUserClans(int userId, string catalog, Action<List<ClanEntity>> successCallback = null, Action<string> failureCallback = null)
		{
			UserHasClan = false;
			_proxy.CallService(Services.GetUserClans, new ClanEntity
			{
				GameId = (int)_gameManager.Game,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				UserId = userId,
				Catalog = catalog,
				MethodType = 4
			}, delegate(string json)
			{
				List<ClanEntity> clans = _proxy.GetEntities<ClanEntity>(json);
				if (clans != null && clans.Count > 0 && clans[0] != null)
				{
					_userClan = clans[0];
					GetClanById(clans[0].ClanId, delegate(ClanEntity userClan)
					{
						UserHasClan = true;
						_userClan = userClan;
						if (successCallback != null)
						{
							successCallback(clans);
						}
					}, delegate(string message)
					{
						Debug.Log("Failed to get user clan info " + message);
					});
				}
			}, failureCallback);
		}

		[Obsolete("No Longer need the user entity", false)]
		public void GetGameClans(UserEntity currentUser, Action<List<ClanEntity>> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetClans, new ClanEntity
			{
				MethodType = 4,
				Version = _gameManager.GameVersion,
				GameId = (int)_gameManager.Game,
				DeviceSerial = _gameManager.DeviceSerial,
				Catalog = currentUser.Catalog,
				UserId = currentUser.UserId
			}, delegate(string clanJson)
			{
				_clans = _proxy.GetEntities<ClanEntity>(clanJson);
				_clanInfo.Clear();
				if (successCallback != null)
				{
					successCallback(_clans);
				}
			}, failureCallback);
		}

		public void GetGameClans(Action<List<ClanEntity>> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetClans, new ClanEntity
			{
				MethodType = 4,
				Version = _gameManager.GameVersion,
				GameId = (int)_gameManager.Game,
				DeviceSerial = _gameManager.DeviceSerial,
				Catalog = UserManager.AnonymousUser.Catalog,
				UserId = UserManager.AnonymousUser.UserId
			}, delegate(string clanJson)
			{
				_clans = _proxy.GetEntities<ClanEntity>(clanJson);
				_clanInfo.Clear();
				if (successCallback != null)
				{
					successCallback(_clans);
				}
			}, failureCallback);
		}

		public void CreateClan(ClanEntity clan, Action<string> successCallback, Action<string> failureCallback)
		{
			_proxy.CallService(Services.CreateClan, clan, delegate(string clanId)
			{
				int? num = Parsers.ParseInt(clanId);
				if (num.HasValue)
				{
					GetClanById(num.Value, delegate(ClanEntity userClan)
					{
						UserClan = userClan;
						if (successCallback != null)
						{
							successCallback(clanId);
						}
					}, delegate(string message)
					{
						if (failureCallback != null)
						{
							failureCallback(message);
						}
					});
				}
			}, failureCallback);
		}

		public void AddUser(int clanId, int userId, string catalog, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.AddUserToClan, new ClanEntity
			{
				ClanId = clanId,
				GameId = (int)_gameManager.Game,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				UserId = userId,
				Catalog = catalog,
				MethodType = 1
			}, delegate(string json)
			{
				Debug.Log("Added the user! " + json);
				if (successCallback != null)
				{
					successCallback(json);
				}
			}, delegate(string message)
			{
				Debug.Log("Failed to add user: " + message);
				if (failureCallback != null)
				{
					failureCallback(message);
				}
			});
		}

		public void RemoveUser(int clanId, int userId, string catalog, Action<string> successCallback = null, Action<string> failurCallback = null)
		{
			RemoveUser(clanId, userId, userId, catalog, delegate(string json)
			{
				_userClan = null;
				if (successCallback != null)
				{
					successCallback(json);
				}
			}, failurCallback);
		}

		public void RemoveUser(int clanId, int userId, int userIdToRemove, string catalog, Action<string> successCallback = null, Action<string> failurCallback = null)
		{
			_proxy.CallService(Services.DeleteUserFromClan, new ClanEntity
			{
				ClanId = clanId,
				GameId = (int)_gameManager.Game,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				UserId = userId,
				Catalog = catalog,
				Members = new List<string> { userIdToRemove.ToString() },
				MethodType = 3
			}, delegate(string json)
			{
				Debug.Log("Removed the user: " + json);
				if (successCallback != null)
				{
					successCallback(json);
				}
			}, delegate(string message)
			{
				Debug.Log("failed to add user: " + message);
				if (failurCallback != null)
				{
					failurCallback(message);
				}
			});
		}

		public void GetClanInvites(int userId, string catalog, Action<List<ClanEntity>> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetClanInvites, new ClanEntity
			{
				MethodType = 4,
				Version = _gameManager.GameVersion,
				DeviceSerial = _gameManager.DeviceSerial,
				GameId = (int)_gameManager.Game,
				UserId = userId,
				Catalog = catalog
			}, delegate(string json)
			{
				_clanInvites = _proxy.GetEntities<ClanEntity>(json);
				if (successCallback != null)
				{
					successCallback(_clanInvites);
				}
			}, delegate(string message)
			{
				if (failureCallback != null)
				{
					failureCallback(message);
				}
			});
		}

		public void SendClanInvite(int senderId, int receiverId, int clanId, string catalog, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.CreateClanInvite, new ClanEntity
			{
				MethodType = 1,
				Version = _gameManager.GameVersion,
				DeviceSerial = _gameManager.DeviceSerial,
				GameId = (int)_gameManager.Game,
				UserId = senderId,
				ClanId = clanId,
				Members = new List<string> { receiverId.ToString() },
				Catalog = catalog
			}, delegate(string message)
			{
				if (successCallback != null)
				{
					successCallback(message);
				}
			}, delegate(string message)
			{
				if (failureCallback != null)
				{
					failureCallback(message);
				}
			});
		}

		public void DeleteClanInvite(int senderId, int receiverId, int clanId, string catalog, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.CreateClanInvite, new ClanEntity
			{
				MethodType = 3,
				Version = _gameManager.GameVersion,
				DeviceSerial = _gameManager.DeviceSerial,
				GameId = (int)_gameManager.Game,
				UserId = senderId,
				ClanId = clanId,
				Members = new List<string> { receiverId.ToString() },
				Catalog = catalog
			}, delegate(string message)
			{
				if (successCallback != null)
				{
					successCallback(message);
				}
			}, delegate(string message)
			{
				if (failureCallback != null)
				{
					failureCallback(message);
				}
			});
		}
	}
}
