using System;
using System.Collections.Generic;

public interface ServiceInterface
{
	string Platform { get; }

	string PrivateMatchPlatforms { get; }

	string SessionId { get; }

	string NameColor { get; }

	bool GotRepReward { get; }

	bool WaitingForResults { get; }

	string LastError { get; }

	string BuildIdentifier { get; }

	string ClientVersion { get; }

	Report.GameReport LastGameReport { get; }

	GameResults LastGameResults { get; }

	string LastAvgWaitTime { get; }

	bool IsPrivateMatch { get; set; }

	event Action<string> SessionIdUpdated;

	void CreateAccount(string user, string pass, bool isGuest, FinishedCallback success, FinishedCallback failure);

	void RequestPasswordReset(string user, FinishedCallback success, FinishedCallback failure);

	void RequestEmailResend(string user, FinishedCallback success, FinishedCallback failure);

	void Login(string user, string pass, FinishedCallback success, FinishedCallback failure);

	void UpgradeFromGuest(string user, string pass, FinishedCallback success, FinishedCallback failure);

	void RefreshItems(FinishedCallback success, FinishedCallback failure);

	void RefreshPlayerLocker(FinishedCallback success, FinishedCallback failure);

	void RefreshPlayerStats(FinishedCallback success, FinishedCallback failure);

	void RefreshStoreItemList(FinishedCallback success, FinishedCallback failure);

	void RefreshServers(FinishedCallback success, FinishedCallback failure);

	void PurchaseDeal(int dealID, FinishedCallback success, FinishedCallback failure);

	void PurchaseItem(Purchaseable pItem, FinishedCallback success, FinishedCallback failure);

	void RequestReward(string rewardName, FinishedCallback success, FinishedCallback failure);

	void RefreshITunesProductList(FinishedCallback success, FinishedCallback failure);

	void RefreshGooglePlayProductList(FinishedCallback success, FinishedCallback failure);

	void RefreshMicrosoftStoreProductList(FinishedCallback success, FinishedCallback failure);

	void PurchaseCurrency(string productID, int quantity, FinishedCallback success, FinishedCallback failure, FinishedCallback cancel);

	void RefillEnergy(FinishedCallback success, FinishedCallback failure);

	void SendReport(Report.GameReport report, FinishedCallback success, FinishedCallback failure);

	void OfferwallTransaction(int amtOfGas, string receiptID, FinishedCallback success, FinishedCallback failure);

	void TradeGasForJoules(int cansPerSet, int numSets, FinishedCallback success, FinishedCallback failure);

	void RequestWaitTime(int index, FinishedCallback success, FinishedCallback failure);

	void RequestGame(int index, FinishedCallback success, FinishedCallback failure);

	void CancelRequestGame();

	void NotifyGameStart(FinishedCallback success, FinishedCallback failure);

	void LogGameLeft(string reason);

	void NotifyPurchaseFailed(string error);

	void NotifyPurchaseCancelled(string error);

	void NotifyPurchaseSuccess(string id, string recipt, int quantity);

	void UpdateForceMatchingServerForPlatform(string platform);

	void CheckMaliciousPlayer(Action<bool> callback);

	List<Server> GetServers(string type);

	List<string> GetTips();

	Stats GetStats();

	Item GetItemByName(string name);

	Item GetItemByName(string name, string character, ref bool hasError);

	Item GetItemByID(int id);

	Item GetDefaultReplacement(Item item);

	IList<Item> GetItemsForCharacterAndType(string character, string type);

	IList<Item> GetItemsForType(string type);

	Dictionary<int, Item> GetLocker();

	Dictionary<string, Item> GetAllItemsByName();

	Dictionary<int, Purchaseable> GetStore();

	Purchaseable GetPurchaseableByID(int id);

	bool IsItemBought(int item_id);

	string GetLastMatchServerUsed();

	string GetPhotonAppID();

	string GetMatchGameServer();

	string GetMatchGameName();

	void SetMatchGameName(string name);

	bool UpdateProperty(string name, ref string val);

	bool UpdateProperty(string name, ref int val);

	bool UpdateProperty(string name, ref float val);

	bool UpdateProperty(string name, ref bool val);

	void OverwriteProperty(string name, string val);

	PictureLink[] GetRotatingPictureLinks();

	int GetGasCansPerPurchase(string productID);

	Dictionary<string, int> GetItunesProducts();

	Dictionary<string, int> GetGooglePlayProducts();

	Dictionary<string, int> GetMicrosoftStoreProducts();

	Dictionary<int, int> GetJouleExchangeRates();

	int GetJoulesExchangeRate(int tradeAmount);

	float getPropertyMax(string propertyName);

	string GetInventoryVersion();

	Reward GetReward(string rewardName);

	bool PlayerHasReward(string rewardName);

	int GetPlayerReward(string rewardName);

	int GetDailyReward();

	List<Deal> GetDeals();

	Deal GetDeal(int dealID);

	string GetServerURL();

	int GetCurrentMatchServerIndex();

	void ReportPlayer(int playerToBeReported, int reportType, FinishedCallback success, FinishedCallback failure);
}
