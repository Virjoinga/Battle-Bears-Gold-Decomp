using System.Collections.Generic;
using SkyVu.Common.Enums;
using SkyVuEngine.SkyVuNetwork.Client.Entities;

namespace SkyVuEngine.SkyVuNetwork.Client
{
	public class ServiceLookup
	{
		public string ServiceUrl { get; set; }

		public ServiceLookup(string serviceUrl)
		{
			ServiceUrl = serviceUrl;
		}

		public ServiceRequest GetServiceUrl(Services service)
		{
			switch (service)
			{
			case Services.TimeStamp:
				return new ServiceRequest(2500, "api/ping", ServiceUrl, true);
			case Services.VerifiyToken:
				return new ServiceRequest(2500, "api/ping", ServiceUrl, true);
			case Services.GetAllUsers:
				return new ServiceRequest(2500, "api/user", ServiceUrl, true);
			case Services.GetUserById:
				return new ServiceRequest(2500, "api/user", ServiceUrl, true);
			case Services.CreateNewUser:
				return new ServiceRequest(2500, "api/user", ServiceUrl, false);
			case Services.GetNewsFeed:
				return new ServiceRequest(2500, "api/newsfeed", ServiceUrl, false);
			case Services.GetNewsFeedsByStartDate:
				return new ServiceRequest(2500, "api/newsfeed", ServiceUrl, false);
			case Services.GetBuddyRequestsForRequester:
				return new ServiceRequest(2500, "api/buddyrequester", ServiceUrl, true);
			case Services.GetBuddyRequestById:
				return new ServiceRequest(2500, "api/buddyrequest", ServiceUrl, true);
			case Services.CreateBuddyRequest:
				return new ServiceRequest(2500, "api/buddyrequest", ServiceUrl, false);
			case Services.UpdateBuddyRequestStat:
				return new ServiceRequest(2500, "api/buddyrequest", ServiceUrl, false);
			case Services.GetBuddyListById:
				return new ServiceRequest(2500, "api/buddylist", ServiceUrl, true);
			case Services.DeleteBuddyList:
				return new ServiceRequest(2500, "api/buddylist", ServiceUrl, false);
			case Services.GetRequestToken:
				return new ServiceRequest(2500, "api/accounttoken", ServiceUrl, true);
			case Services.GetRankingSystemCategory_1:
				return new ServiceRequest(2500, "api/RankingSystemCategory_1", ServiceUrl, true);
			case Services.CreateRankingSystemCategory_1:
				return new ServiceRequest(2500, "api/RankingSystemCategory_1", ServiceUrl, false);
			case Services.UpdateRankingSystemCategory_1:
				return new ServiceRequest(2500, "api/RankingSystemCategory_1", ServiceUrl, false);
			case Services.GetCategory:
				return new ServiceRequest(2500, "api/Category", ServiceUrl, false);
			case Services.CreateCategory:
				return new ServiceRequest(2500, "api/Category", ServiceUrl, false);
			case Services.IapVerify:
				return new ServiceRequest(2500, "api/IapVerify", ServiceUrl, false);
			case Services.GetPatches:
				return new ServiceRequest(2500, "api/Patch", ServiceUrl, true);
			case Services.GetUserByCategory:
				return new ServiceRequest(2500, "api/UserSearch", ServiceUrl, false);
			case Services.UpdateUser:
				return new ServiceRequest(2500, "api/User", ServiceUrl, false);
			case Services.GetMessages:
				return new ServiceRequest(2500, "api/Message", ServiceUrl, false);
			case Services.CreateMessage:
				return new ServiceRequest(2500, "api/Message", ServiceUrl, false);
			case Services.SetMessageRead:
				return new ServiceRequest(2500, "api/Message", ServiceUrl, false);
			case Services.DeleteMessage:
				return new ServiceRequest(2500, "api/Message", ServiceUrl, false);
			case Services.DataTracking:
				return new ServiceRequest(2500, "api/DataTracking", ServiceUrl, false);
			case Services.UpdateAccount:
				return new ServiceRequest(2500, "api/Account", ServiceUrl, false);
			case Services.GetSalesOffer:
				return new ServiceRequest(2500, "api/UserSaleOffer", ServiceUrl, false);
			case Services.GetBatchedUserData:
				return new ServiceRequest(2500, "api/BatchedUserData", ServiceUrl, false);
			case Services.MakePurchase:
				return new ServiceRequest(2500, "api/Purchase", ServiceUrl, false);
			case Services.GetCurrencyConversion:
				return new ServiceRequest(2500, "api/GameCC", ServiceUrl, true);
			case Services.GetGameItemAttributes:
				return new ServiceRequest(2500, "api/GameItemAttribute", ServiceUrl, true);
			case Services.GetGameItems:
				return new ServiceRequest(2500, "api/GameItem", ServiceUrl, true);
			case Services.GetGameSales:
				return new ServiceRequest(2500, "api/GameSale", ServiceUrl, true);
			case Services.GameIapPackages:
				return new ServiceRequest(2500, "api/IapPackage", ServiceUrl, true);
			case Services.GetUserE:
				return new ServiceRequest(2500, "api/UserE", ServiceUrl, true);
			case Services.CreateUserE:
				return new ServiceRequest(2500, "api/UserE", ServiceUrl, false);
			case Services.UpdateUserE:
				return new ServiceRequest(2500, "api/UserE", ServiceUrl, false);
			case Services.GetGameSettings:
				return new ServiceRequest(2500, "api/GameSettings", ServiceUrl, true);
			case Services.GetUserGameItems:
				return new ServiceRequest(2500, "api/UserGameItem", ServiceUrl, true);
			case Services.CreateUserGameItems:
				return new ServiceRequest(2500, "api/UserGameItem", ServiceUrl, false);
			case Services.UpdateUserGameItems:
				return new ServiceRequest(2500, "api/UserGameItem", ServiceUrl, false);
			case Services.GetBundles:
				return new ServiceRequest(2500, "api/Bundle", ServiceUrl, true);
			case Services.GetBundleItems:
				return new ServiceRequest(2500, "api/BundleItem", ServiceUrl, true);
			case Services.GetUserCustomData:
				return new ServiceRequest(2500, "api/UserCustomData", ServiceUrl, true);
			case Services.CreateUserCustomData:
				return new ServiceRequest(2500, "api/UserCustomData", ServiceUrl, false);
			case Services.GetCustomGameData:
				return new ServiceRequest(2500, "api/CustomGameData", ServiceUrl, false);
			case Services.SendGameMessage:
				return new ServiceRequest(2500, "api/GameMessage", ServiceUrl, false);
			case Services.SendGameMessageBBG:
				return new ServiceRequest(2500, "api/GameMessageBBG", ServiceUrl, false);
			case Services.GetRetention:
				return new ServiceRequest(2500, "api/Retention", ServiceUrl, false);
			case Services.CreateMessageBan:
				return new ServiceRequest(2500, "api/MessageBan", ServiceUrl, false);
			case Services.DeleteMessageBan:
				return new ServiceRequest(2500, "api/MessageBan", ServiceUrl, false);
			case Services.GetMessageBans:
				return new ServiceRequest(2500, "api/MessageBan", ServiceUrl, false);
			case Services.GetAchievement:
				return new ServiceRequest(2500, "api/Achievement", ServiceUrl, true);
			case Services.GetLeaderBoard:
				return new ServiceRequest(2500, "api/LeaderBoard", ServiceUrl, true);
			case Services.GetUserAchievement:
				return new ServiceRequest(2500, "api/UserAchievement", ServiceUrl, true);
			case Services.CreateUserAchievement:
				return new ServiceRequest(2500, "api/UserAchievement", ServiceUrl, false);
			case Services.GetUserLeaderBoard:
				return new ServiceRequest(2500, "api/UserLeaderBoard", ServiceUrl, true);
			case Services.CreateUserLeaderBoard:
				return new ServiceRequest(2500, "api/UserLeaderBoard", ServiceUrl, false);
			case Services.UpdateUserLeaderBoard:
				return new ServiceRequest(2500, "api/UserLeaderBoard", ServiceUrl, false);
			case Services.GetLeaderboardUsers:
				return new ServiceRequest(2500, "api/LeaderBoardUsers", ServiceUrl, false);
			case Services.GetClanById:
				return new ServiceRequest(2500, "api/Clan", ServiceUrl, true);
			case Services.GetClans:
				return new ServiceRequest(2500, "api/Clan", ServiceUrl, false);
			case Services.CreateClan:
				return new ServiceRequest(2500, "api/Clan", ServiceUrl, false);
			case Services.AddUserToClan:
				return new ServiceRequest(2500, "api/UserClan", ServiceUrl, false);
			case Services.DeleteUserFromClan:
				return new ServiceRequest(2500, "api/UserClan", ServiceUrl, false);
			case Services.GetUserClans:
				return new ServiceRequest(2500, "api/UserClan", ServiceUrl, false);
			case Services.GetClanInvites:
				return new ServiceRequest(2500, "api/ClanInvite", ServiceUrl, false);
			case Services.CreateClanInvite:
				return new ServiceRequest(2500, "api/ClanInvite", ServiceUrl, false);
			case Services.DeleteClanInvite:
				return new ServiceRequest(2500, "api/ClanInvite", ServiceUrl, false);
			default:
				return null;
			}
		}

		public T GetEntity<T>(string json) where T : BaseEntity, new()
		{
			T result = new T();
			if (result.Populate(json))
			{
				return result;
			}
			return (T)null;
		}

		public List<T> GetEntities<T>(string json) where T : BaseEntity, new()
		{
			if (string.IsNullOrEmpty(json) || json == "null" || !json.StartsWith("["))
			{
				return null;
			}
			List<T> list = new List<T>();
			json = json.Remove(json.Length - 2, 2);
			string[] array = json.Split('}');
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Remove(0, 1);
				T entity = GetEntity<T>(array[i] + "}");
				if (entity != null)
				{
					list.Add(entity);
					continue;
				}
				return null;
			}
			return list;
		}
	}
}
