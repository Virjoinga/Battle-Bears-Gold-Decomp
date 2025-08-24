using System;
using System.Collections.Generic;
using SkyVu.Common.Enums;
using SkyVuEngine.Core;
using SkyVuEngine.SkyVuNetwork.Client.Entities;

namespace SkyVuEngine.SkyVuNetwork.Client.Managers
{
	public class AchievementManager
	{
		private GameManager _gameManager = null;

		private ISkyVuNetworkProxy _proxy = null;

		private UserEntity _userEntity = null;

		private List<AchievementEntity> _gameAchievements = null;

		private List<UserAchievementEntity> _userAchievements = null;

		private bool _isGameAchievementsFinished = false;

		public Action<List<AchievementEntity>> SuccessCallback { get; set; }

		public Action<string> FailureCallback { get; set; }

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

		public AchievementManager(ISkyVuNetworkProxy proxy, GameManager gameManager, UserEntity userEntity)
		{
			_gameManager = gameManager;
			_proxy = proxy;
			_userEntity = userEntity;
			SuccessCallback = null;
			FailureCallback = null;
		}

		public bool GetGameAchivements(ref List<AchievementEntity> gameAchievements)
		{
			gameAchievements = _gameAchievements;
			return _isGameAchievementsFinished;
		}

		public void AssignGameAchievements(List<AchievementEntity> gameAchievements)
		{
			_gameAchievements = new List<AchievementEntity>(gameAchievements);
			_isGameAchievementsFinished = true;
		}

		public void AssignUserAchievements(List<UserAchievementEntity> userAchievements)
		{
			_userAchievements = userAchievements;
		}

		public void PopulateGameAchievements()
		{
			_proxy.CallService(Services.GetAchievement, new AchievementEntity
			{
				GameId = (int)_gameManager.Game,
				AchievementId = 0,
				Catalog = UserManager.AnonymousUser.Catalog,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				_isGameAchievementsFinished = true;
				_gameAchievements = _proxy.GetEntities<AchievementEntity>(json);
				if (SuccessCallback != null)
				{
					SuccessCallback(_gameAchievements);
				}
			}, delegate(string json)
			{
				_isGameAchievementsFinished = true;
				if (FailureCallback != null)
				{
					FailureCallback(json);
				}
			});
		}

		public void GetUserAchievements(Action<List<UserAchievementEntity>> successCallback = null, Action<string> failureCallback = null)
		{
			GetUserAchievements(_userEntity, successCallback, failureCallback);
		}

		public void GetUserAchievements(UserEntity userEntity, Action<List<UserAchievementEntity>> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetUserAchievement, new UserAchievementEntity
			{
				GameId = (int)_gameManager.Game,
				AchievementId = 0,
				UserAchievementId = 0,
				UserId = userEntity.UserId,
				Catalog = userEntity.Catalog,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				_userAchievements = _proxy.GetEntities<UserAchievementEntity>(json);
				if (successCallback != null)
				{
					successCallback(_userAchievements);
				}
			}, delegate(string json)
			{
				if (failureCallback != null)
				{
					failureCallback(json);
				}
			});
		}

		public void AddUserAchievement(int achievementId, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.CreateUserAchievement, new UserAchievementEntity
			{
				GameId = (int)_gameManager.Game,
				AchievementId = achievementId,
				UserAchievementId = 0,
				UserId = _userEntity.UserId,
				Catalog = _userEntity.Catalog,
				MethodType = 1,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				if (_userAchievements == null)
				{
					_userAchievements = new List<UserAchievementEntity>();
				}
				_userAchievements.Add(new UserAchievementEntity
				{
					AchievementId = achievementId,
					UserId = _userEntity.UserId,
					GameId = (int)_gameManager.Game,
					CreateDate = DateTime.Now.ToShortDateString()
				});
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

		public void AddUserAchievement(UserEntity userEntity, int achievementId, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.CreateUserAchievement, new UserAchievementEntity
			{
				GameId = (int)_gameManager.Game,
				AchievementId = achievementId,
				UserAchievementId = 0,
				UserId = userEntity.UserId,
				Catalog = userEntity.Catalog,
				MethodType = 1,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				if (_userAchievements == null)
				{
					_userAchievements = new List<UserAchievementEntity>();
				}
				_userAchievements.Add(new UserAchievementEntity
				{
					AchievementId = achievementId,
					UserId = userEntity.UserId,
					GameId = (int)_gameManager.Game,
					CreateDate = DateTime.Now.ToShortDateString()
				});
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

		public int GetAchievementId(string achievementLabel)
		{
			int result = -1;
			AchievementEntity achievementEntity = _gameAchievements.Find((AchievementEntity obj) => obj.LabelName == achievementLabel);
			if (achievementEntity != null)
			{
				result = achievementEntity.AchievementId;
			}
			return result;
		}

		public bool UserHasAchievement(string achievementLabel)
		{
			int achievementId = GetAchievementId(achievementLabel);
			return UserHasAchievement(achievementId);
		}

		public bool UserHasAchievement(int achievementId)
		{
			bool result = false;
			if (achievementId > -1 && _userAchievements != null)
			{
				result = _userAchievements.Find((UserAchievementEntity obj) => obj.AchievementId == achievementId) != null;
			}
			return result;
		}
	}
}
