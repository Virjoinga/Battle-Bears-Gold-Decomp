using System;
using System.Collections.Generic;
using SkyVu.Common.Enums;
using SkyVuEngine.Core;
using SkyVuEngine.SkyVuNetwork.Client.Entities;

namespace SkyVuEngine.SkyVuNetwork.Client.Managers
{
	public class SettingsManager
	{
		private GameManager _gameManager = null;

		private ISkyVuNetworkProxy _proxy = null;

		private List<GameSettingEntity> _gameSettings = null;

		private bool _isGameSettingsLoaded = false;

		private List<GameSaleEntity> _gameSales = null;

		private bool _isGameSalesLoaded = false;

		private List<IapPackageEntity> _iapPackages = null;

		private bool _isIapPackagesLoaded = false;

		private List<GameCurrencyConversionEntity> _gameCurrencyConversions = null;

		private bool _isGameCurrencyConversionsLoaded = false;

		private List<PatchEntity> _patches = null;

		private bool _isPatchesLoaded = false;

		private List<BundleEntity> _bundles = null;

		private bool _isBundlesLoaded = false;

		private List<NewsFeedEntity> _newsFeeds = null;

		private bool _isNewsFeedsLoaded = false;

		private int _newsFeedsLimit;

		public Action<string> SuccessCallback { get; set; }

		public Action<string> FailureCallback { get; set; }

		public SettingsManager(ISkyVuNetworkProxy proxy, GameManager gameManager, int newsFeedsLimit)
		{
			_gameManager = gameManager;
			_proxy = proxy;
			SuccessCallback = null;
			FailureCallback = null;
			_newsFeedsLimit = newsFeedsLimit;
		}

		public bool GetGameSettings(ref List<GameSettingEntity> gameSettings)
		{
			gameSettings = _gameSettings;
			return _isGameSettingsLoaded;
		}

		public bool GetGameSales(ref List<GameSaleEntity> gameSales)
		{
			gameSales = _gameSales;
			return _isGameSalesLoaded;
		}

		public bool GetGameSales(ref List<IapPackageEntity> gameSales)
		{
			gameSales = _iapPackages;
			return _isIapPackagesLoaded;
		}

		public bool GetCurrencyConversion(ref List<GameCurrencyConversionEntity> currencyConversionEntities)
		{
			currencyConversionEntities = _gameCurrencyConversions;
			return _isGameCurrencyConversionsLoaded;
		}

		public bool GetPatches(ref List<PatchEntity> patchEntities)
		{
			patchEntities = _patches;
			return _isPatchesLoaded;
		}

		public bool GetBundles(ref List<BundleEntity> bundleEntities)
		{
			bundleEntities = _bundles;
			return _isBundlesLoaded;
		}

		public bool GetNewsFeeds(ref List<NewsFeedEntity> newsFeedsEntities)
		{
			newsFeedsEntities = _newsFeeds;
			return _isNewsFeedsLoaded;
		}

		public void PopulateBundleItems(int bundleId, Action<string> successCallback)
		{
			_proxy.CallService(Services.GetBundleItems, new BundleItemEntity
			{
				GameItemId = (int)_gameManager.Game,
				Catalog = UserManager.AnonymousUser.Catalog,
				UserId = UserManager.AnonymousUser.UserId,
				BundleId = bundleId,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				if (successCallback != null)
				{
					successCallback(json);
				}
			}, delegate(string json)
			{
				if (FailureCallback != null)
				{
					FailureCallback(json);
				}
			});
		}

		public void UpdateSettings()
		{
			PopulateGameSettings();
			PopulateGameSales();
			PopulateIapPackages();
			PopulateCurrencyConversion();
			PopulatePatches();
		}

		public void PopulateSettings()
		{
			PopulateGameSettings();
			PopulateGameSales();
			PopulateIapPackages();
			PopulateCurrencyConversion();
			PopulatePatches();
			PopulateBundles();
			PopulateNewsFeeds(_newsFeedsLimit);
		}

		public void PopulateSettings(CompiledGameDataEntity gameData)
		{
			_gameSettings = new List<GameSettingEntity>(gameData.GameSettings);
			_isGameSettingsLoaded = true;
			_gameSales = new List<GameSaleEntity>(gameData.GameSales);
			_isGameSalesLoaded = true;
			_iapPackages = new List<IapPackageEntity>(gameData.IapPackages);
			_isIapPackagesLoaded = true;
			_gameCurrencyConversions = new List<GameCurrencyConversionEntity>(gameData.GameCurrencyConversions);
			_isGameCurrencyConversionsLoaded = true;
			_patches = new List<PatchEntity>(gameData.Patches);
			_isPatchesLoaded = true;
			_bundles = new List<BundleEntity>(gameData.Bundles);
			_isBundlesLoaded = true;
			_newsFeeds = new List<NewsFeedEntity>(gameData.NewsFeeds);
			_isNewsFeedsLoaded = true;
		}

		private void PopulateGameSettings()
		{
			_proxy.CallService(Services.GetGameSettings, new GameSettingEntity
			{
				GameId = (int)_gameManager.Game,
				Catalog = UserManager.AnonymousUser.Catalog,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				_isGameSettingsLoaded = true;
				_gameSettings = _proxy.GetEntities<GameSettingEntity>(json);
				if (SuccessCallback != null)
				{
					SuccessCallback(json);
				}
			}, delegate(string json)
			{
				_isGameSettingsLoaded = true;
				if (FailureCallback != null)
				{
					FailureCallback(json);
				}
			});
		}

		private void PopulateGameSales()
		{
			_proxy.CallService(Services.GetGameSales, new GameSaleEntity
			{
				GameId = (int)_gameManager.Game,
				Catalog = UserManager.AnonymousUser.Catalog,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				_isGameSalesLoaded = true;
				_gameSales = _proxy.GetEntities<GameSaleEntity>(json);
				if (SuccessCallback != null)
				{
					SuccessCallback(json);
				}
			}, delegate(string json)
			{
				_isGameSalesLoaded = true;
				if (FailureCallback != null)
				{
					FailureCallback(json);
				}
			});
		}

		private void PopulateIapPackages()
		{
			_proxy.CallService(Services.GameIapPackages, new IapPackageEntity
			{
				GameId = (int)_gameManager.Game,
				Catalog = UserManager.AnonymousUser.Catalog,
				UserId = UserManager.AnonymousUser.UserId,
				ProductLabel = ".",
				Description = ".",
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				_isIapPackagesLoaded = true;
				_iapPackages = _proxy.GetEntities<IapPackageEntity>(json);
				if (SuccessCallback != null)
				{
					SuccessCallback(json);
				}
			}, delegate(string json)
			{
				_isIapPackagesLoaded = true;
				if (FailureCallback != null)
				{
					FailureCallback(json);
				}
			});
		}

		private void PopulateCurrencyConversion()
		{
			_proxy.CallService(Services.GetCurrencyConversion, new GameCurrencyConversionEntity
			{
				GameId = (int)_gameManager.Game,
				Catalog = UserManager.AnonymousUser.Catalog,
				UserId = UserManager.AnonymousUser.UserId,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				_isGameCurrencyConversionsLoaded = true;
				_gameCurrencyConversions = _proxy.GetEntities<GameCurrencyConversionEntity>(json);
				if (SuccessCallback != null)
				{
					SuccessCallback(json);
				}
			}, delegate(string json)
			{
				_isGameCurrencyConversionsLoaded = true;
				if (FailureCallback != null)
				{
					FailureCallback(json);
				}
			});
		}

		public void PopulatePatches()
		{
			_proxy.CallService(Services.GetPatches, new PatchEntity
			{
				GameId = (int)_gameManager.Game,
				Catalog = UserManager.AnonymousUser.Catalog,
				UserId = UserManager.AnonymousUser.UserId,
				GameVersion = _gameManager.PatchVersion,
				GamePlatformType = _gameManager.GamePlatform,
				Version = 1,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				_isPatchesLoaded = true;
				_patches = _proxy.GetEntities<PatchEntity>(json);
				if (SuccessCallback != null)
				{
					SuccessCallback(json);
				}
			}, delegate(string json)
			{
				_isPatchesLoaded = true;
				if (FailureCallback != null)
				{
					FailureCallback(json);
				}
			});
		}

		private void PopulateBundles()
		{
			_proxy.CallService(Services.GetBundles, new BundleEntity
			{
				GameId = (int)_gameManager.Game,
				Catalog = UserManager.AnonymousUser.Catalog,
				UserId = UserManager.AnonymousUser.UserId,
				DeviceSerial = _gameManager.DeviceSerial
			}, delegate(string json)
			{
				_isBundlesLoaded = true;
				_bundles = _proxy.GetEntities<BundleEntity>(json);
				if (SuccessCallback != null)
				{
					SuccessCallback(json);
				}
			}, delegate(string json)
			{
				_isBundlesLoaded = true;
				if (FailureCallback != null)
				{
					FailureCallback(json);
				}
			});
		}

		private void PopulateNewsFeeds(int limit)
		{
			_proxy.CallService(Services.GetNewsFeed, new NewsFeedEntity
			{
				MethodType = 4,
				Catalog = UserManager.AnonymousUser.Catalog,
				UserId = UserManager.AnonymousUser.UserId,
				Limit = limit,
				DeviceSerial = _gameManager.DeviceSerial,
				Version = _gameManager.GameVersion
			}, delegate(string json)
			{
				_isNewsFeedsLoaded = true;
				_newsFeeds = _proxy.GetEntities<NewsFeedEntity>(json);
				if (SuccessCallback != null)
				{
					SuccessCallback(json);
				}
			}, delegate(string json)
			{
				_isNewsFeedsLoaded = true;
				if (FailureCallback != null)
				{
					FailureCallback(json);
				}
			});
		}
	}
}
