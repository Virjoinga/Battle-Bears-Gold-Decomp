using System;
using SkyVu.Common;
using SkyVu.Common.Enums;
using SkyVuEngine.Core;
using SkyVuEngine.SkyVuNetwork.Client.Entities;

namespace SkyVuEngine.SkyVuNetwork.Client.Managers
{
	public class PurchasingManager
	{
		private ISkyVuNetworkProxy _proxy;

		private GameManager _game;

		public PurchasingManager(ISkyVuNetworkProxy proxy, GameManager gameManager)
		{
			_proxy = proxy;
			_game = gameManager;
		}

		public void PurchaseGameItem(UserEntity currentUser, int gameItemId, int count, Action<int> success = null, Action<string> failure = null)
		{
			_proxy.CallService(Services.MakePurchase, new UserGameItemEntity
			{
				Version = _game.GameVersion,
				GameId = (int)_game.Game,
				DeviceSerial = _game.DeviceSerial,
				UserId = currentUser.UserId,
				Catalog = currentUser.Catalog,
				GameItemId = gameItemId,
				Count = count,
				UserGameItemId = 1,
				MethodType = 1
			}, delegate(string successCode)
			{
				int? num = Parsers.ParseInt(successCode);
				if (success != null)
				{
					success(num.Value);
				}
			}, delegate(string message)
			{
				if (failure != null)
				{
					failure(message);
				}
			});
		}
	}
}
