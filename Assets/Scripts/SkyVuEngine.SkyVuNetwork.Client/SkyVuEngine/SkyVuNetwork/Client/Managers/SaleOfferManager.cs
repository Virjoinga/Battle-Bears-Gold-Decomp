using System;
using SkyVu.Common.Enums;
using SkyVuEngine.Core;
using SkyVuEngine.SkyVuNetwork.Client.Entities;

namespace SkyVuEngine.SkyVuNetwork.Client.Managers
{
	public class SaleOfferManager
	{
		private SaleOfferEntity _currentOffer;

		private ISkyVuNetworkProxy _proxy;

		private GameManager _gameManager;

		public SaleOfferEntity CurrentOffer
		{
			get
			{
				return _currentOffer;
			}
			private set
			{
				_currentOffer = value;
			}
		}

		public bool IsOutdated { get; set; }

		public SaleOfferManager(ISkyVuNetworkProxy proxy, GameManager gameManager)
		{
			_proxy = proxy;
			_gameManager = gameManager;
			CurrentOffer = null;
		}

		public void GetSalesOffer(int userId, string catalog, Action<SaleOfferEntity> success = null, Action<string> failure = null)
		{
			_proxy.CallService(Services.GetSalesOffer, new SaleOfferEntity
			{
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion,
				UserId = userId,
				GameId = (int)_gameManager.Game,
				Catalog = catalog,
				MethodType = 4
			}, delegate(string json)
			{
				if (json == null)
				{
					if (failure != null)
					{
						failure("No sale for the user");
					}
				}
				else
				{
					CurrentOffer = _proxy.GetEntity<SaleOfferEntity>(json);
					IsOutdated = false;
					if (CurrentOffer != null && success != null)
					{
						success(CurrentOffer);
					}
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
