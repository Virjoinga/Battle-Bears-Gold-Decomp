using System;
using System.Collections.Generic;
using SkyVu.Common.Enums;
using SkyVuEngine.Core;
using SkyVuEngine.SkyVuNetwork.Client.Entities;
using UnityEngine;

namespace SkyVuEngine.SkyVuNetwork.Client.Managers
{
	public class LeaderBoardsManager
	{
		public class LeaderBoard
		{
			public int LeaderBoardId { get; set; }

			public int GameId { get; set; }

			public int GameTypeId { get; set; }

			public string Title { get; set; }

			public int ScoreMin { get; set; }

			public int ScoreMax { get; set; }

			public int ScoreUnitTypeId { get; set; }

			public LeaderBoard(LeaderBoardEntity leaderBoardEntity)
			{
				LeaderBoardId = leaderBoardEntity.LeaderBoardId;
				GameId = leaderBoardEntity.GameId;
				GameTypeId = leaderBoardEntity.GameTypeId;
				Title = leaderBoardEntity.Title;
				ScoreMin = leaderBoardEntity.ScoreMin;
				ScoreMax = leaderBoardEntity.ScoreMax;
				ScoreUnitTypeId = leaderBoardEntity.ScoreUnitTypeId;
			}
		}

		private GameManager _gameManager = null;

		private ISkyVuNetworkProxy _proxy = null;

		private Dictionary<LeaderBoardEntity, List<UserLeaderBoardEntity>> _leaderBoards;

		public Action<List<GameItemEntity>> SuccessCallback { get; set; }

		public Action<string> FailureCallback { get; set; }

		public Dictionary<LeaderBoardEntity, List<UserLeaderBoardEntity>> LeaderBoards
		{
			get
			{
				return _leaderBoards;
			}
		}

		public LeaderBoardsManager(ISkyVuNetworkProxy proxy, GameManager gameManager)
		{
			_gameManager = gameManager;
			_proxy = proxy;
			SuccessCallback = null;
			FailureCallback = null;
			_leaderBoards = new Dictionary<LeaderBoardEntity, List<UserLeaderBoardEntity>>();
		}

		public void SetLeaderBoards(List<LeaderBoardEntity> leaderBoards)
		{
			_leaderBoards.Clear();
			foreach (LeaderBoardEntity leaderBoard in leaderBoards)
			{
				_leaderBoards.Add(leaderBoard, null);
			}
		}

		public void SetUserLeaderBoards(List<UserLeaderBoardEntity> userLeaderBoards)
		{
			userLeaderBoards.ForEach(delegate(UserLeaderBoardEntity userLeaderBoard)
			{
				foreach (KeyValuePair<LeaderBoardEntity, List<UserLeaderBoardEntity>> leaderBoard in _leaderBoards)
				{
					if (leaderBoard.Key.LeaderBoardId != userLeaderBoard.LeaderBoardId)
					{
					}
				}
			});
		}

		public void GetLeaderBoards(UserEntity userEntity, Action<List<LeaderBoardEntity>> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetLeaderBoard, new LeaderBoardEntity
			{
				GameId = (int)_gameManager.Game,
				UserId = userEntity.UserId,
				Catalog = userEntity.Catalog,
				DeviceSerial = _gameManager.DeviceSerial,
				GameTypeId = 1,
				ScoreUnitTypeId = 1,
				Title = "."
			}, delegate(string json)
			{
				List<LeaderBoardEntity> entities = _proxy.GetEntities<LeaderBoardEntity>(json);
				if (entities != null)
				{
					SetLeaderBoards(entities);
				}
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
			});
		}

		public void GetUsersLeaderBoard(UserEntity userEntity, int id, Action<List<UserLeaderBoardEntity>> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.GetUserLeaderBoard, new UserLeaderBoardEntity
			{
				GameId = (int)_gameManager.Game,
				LeaderBoardId = id,
				DeviceSerial = _gameManager.DeviceSerial,
				UserId = userEntity.UserId,
				Catalog = userEntity.Catalog,
				MethodType = 4
			}, delegate(string json)
			{
				List<UserLeaderBoardEntity> entities = _proxy.GetEntities<UserLeaderBoardEntity>(json);
				if (entities != null)
				{
					foreach (KeyValuePair<LeaderBoardEntity, List<UserLeaderBoardEntity>> leaderBoard in _leaderBoards)
					{
						if (leaderBoard.Key.LeaderBoardId == id)
						{
							_leaderBoards[leaderBoard.Key] = new List<UserLeaderBoardEntity>(entities);
							break;
						}
					}
				}
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
			});
		}

		public void CreateUserLeaderBoards(UserEntity userEntity, int id, int score, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.CreateUserLeaderBoard, new UserLeaderBoardEntity
			{
				GameId = (int)_gameManager.Game,
				DeviceSerial = _gameManager.DeviceSerial,
				LeaderBoardId = id,
				Score = score,
				UserId = userEntity.UserId,
				Catalog = userEntity.Catalog,
				MethodType = 1
			}, delegate(string json)
			{
				UserLeaderBoardEntity entity = _proxy.GetEntity<UserLeaderBoardEntity>(json);
				if (entity != null)
				{
					foreach (KeyValuePair<LeaderBoardEntity, List<UserLeaderBoardEntity>> leaderBoard in _leaderBoards)
					{
						if (leaderBoard.Key.LeaderBoardId == id)
						{
							_leaderBoards[leaderBoard.Key].Add(entity);
							break;
						}
					}
				}
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

		public void UpdateUserLeaderBoard(UserEntity userEntity, int id, int score, Action<string> successCallback = null, Action<string> failureCallback = null)
		{
			_proxy.CallService(Services.UpdateUserLeaderBoard, new UserLeaderBoardEntity
			{
				GameId = (int)_gameManager.Game,
				DeviceSerial = _gameManager.DeviceSerial,
				LeaderBoardId = id,
				Score = score,
				UserId = userEntity.UserId,
				Catalog = userEntity.Catalog,
				MethodType = 2
			}, delegate(string json)
			{
				Debug.LogWarning("LeaderboardManager: TODO update the local dictionary of leader boards " + json);
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

		public void GetUserRating(UserEntity user, Action<string> successfulCallBack = null, Action<string> failureCallBack = null)
		{
			_proxy.CallService(Services.CreateRankingSystemCategory_1, new RankingSystemEntity
			{
				UserId = user.UserId,
				Catalog = user.Catalog,
				RankingId = 0,
				Value = 0
			}, successfulCallBack, failureCallBack);
		}
	}
}
