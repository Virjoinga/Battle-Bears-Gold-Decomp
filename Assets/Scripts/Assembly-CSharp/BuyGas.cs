using System.Collections;
using System.Collections.Generic;
using Analytics;
using Analytics.Parameters;
using Analytics.Schemas;
using Prime31;
using UnityEngine;

public class BuyGas : Popup
{
	private bool buyingGas;

	public GameObject contactingAppleOverlay;

	public GameObject contactingFailedOverlay;

	public GameObject cancelOverlay;

	private GameObject currentContactOverlay;

	private GameObject currentFailedOverlay;

	private GameObject currentCancelOverlay;

	public IAPButtonArrangerWithFreeGas buttonArranger;

	private int unitsBought;

	private int purchasedProductIndex;

	private string lastPurchaseType;

	public AudioClip[] clickSounds;

	public TextMesh currentGasDisplay;

	public static string lastPurchaseProductID = string.Empty;

	public string ValueOfPurchase
	{
		get
		{
			return buttonArranger.Buttons[purchasedProductIndex].RealMoneyPrice;
		}
	}

	public string CurrencyCode
	{
		get
		{
			return buttonArranger.Buttons[purchasedProductIndex].CurrencyCode;
		}
	}

	public int NumberOfGasCans
	{
		get
		{
			if (buttonArranger.Buttons[purchasedProductIndex].CurrencyType == IAPButtonCurrencyTypes.GasCans)
			{
				return unitsBought * buttonArranger.Buttons[purchasedProductIndex].InGameCurrencyValue;
			}
			return 0;
		}
	}

	protected override void Start()
	{
		currentGasDisplay.text = string.Format("{0:#,0}", ServiceManager.Instance.GetStats().gas);
		SetupButtonArranger();
		EventTracker.TrackEvent(new GetGasOpenedSchema());
		if (Store.Instance.StoreProducts.Count > 0)
		{
		}
	}

	private IEnumerator RefreshStoreProductsFromStore()
	{
		currentContactOverlay = Object.Instantiate(contactingAppleOverlay) as GameObject;
		yield return new WaitForSeconds(15f);
		Object.Destroy(currentContactOverlay);
		StartCoroutine(showErrorPopup());
		yield return new WaitForSeconds(2f);
		OnClose();
	}

	public void StoreProductsRefreshed()
	{
		StopCoroutine("RefreshStoreProductsFromStore");
		SetupButtonArranger();
	}

	private void SetupButtonArranger()
	{
		Dictionary<string, int> googlePlayProducts = ServiceManager.Instance.GetGooglePlayProducts();
		List<KeyValuePair<string, int>> gasCanProductsSorted = GetGasCanProductsSorted(googlePlayProducts);
		string bestDeal = GetBestDeal(gasCanProductsSorted);
		int num = 0;
		foreach (KeyValuePair<string, int> item in gasCanProductsSorted)
		{
			buttonArranger.Buttons[num].IAPProductID = item.Key;
			buttonArranger.Buttons[num].CurrencyType = ((!item.Key.Contains("gascan")) ? IAPButtonCurrencyTypes.Joules : IAPButtonCurrencyTypes.GasCans);
			buttonArranger.Buttons[num].InGameCurrencyValue = item.Value;
			buttonArranger.Buttons[num].BestDeal = item.Key == bestDeal;
			GoogleSkuInfo storeProduct = Store.Instance.getStoreProduct(item.Key);
			if (storeProduct != null)
			{
				buttonArranger.Buttons[num].RealMoneyPrice = storeProduct.price;
				buttonArranger.Buttons[num].CurrencyCode = storeProduct.priceCurrencyCode;
			}
			num++;
		}
		for (int i = num; i < buttonArranger.Buttons.Length; i++)
		{
			buttonArranger.Buttons[i].Disabled = true;
		}
	}

	private List<KeyValuePair<string, int>> GetGasCanProductsSorted(Dictionary<string, int> productIds)
	{
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
		if (productIds != null)
		{
			foreach (KeyValuePair<string, int> productId in productIds)
			{
				if (productId.Key.Contains("gascan"))
				{
					list.Add(productId);
				}
			}
			list.Sort((KeyValuePair<string, int> x, KeyValuePair<string, int> y) => x.Value.CompareTo(y.Value));
		}
		return list;
	}

	private string GetBestDeal(List<KeyValuePair<string, int>> productIds)
	{
		float num = 1000000f;
		string result = string.Empty;
		for (int i = 1; i < productIds.Count; i++)
		{
			GoogleSkuInfo storeProduct = Store.Instance.getStoreProduct(productIds[i].Key);
			float result2 = 1000000f;
			if (storeProduct != null)
			{
				float.TryParse(storeProduct.price, out result2);
			}
			float num2 = result2 / (float)productIds[i].Value;
			storeProduct = Store.Instance.getStoreProduct(productIds[i - 1].Key);
			result2 = 1000000f;
			if (storeProduct != null)
			{
				float.TryParse(storeProduct.price, out result2);
			}
			float num3 = result2 / (float)productIds[i - 1].Value;
			float num4 = num2 / num3;
			if (num4 < num)
			{
				num = num4;
				result = productIds[i].Key;
			}
		}
		return result;
	}

	private void ReportPurchase(bool boughtStuff)
	{
		if (!boughtStuff)
		{
		}
	}

	private void ReportAggregatePurchase()
	{
	}

	private void beginPurchaseProcess(string purchaseType)
	{
		lastPurchaseType = purchaseType;
		MainMenu mainMenu = Object.FindObjectOfType(typeof(MainMenu)) as MainMenu;
		if (mainMenu != null)
		{
			mainMenu.currentPopup = null;
			mainMenu.TryCreatePopup(new SetupPopup(mainMenu.guestGasWarningPopup, SetupGuestGasWarningPopup));
		}
		else
		{
			makePurchase(purchaseType);
		}
	}

	private void SetupGuestGasWarningPopup(GameObject warningPopup)
	{
		StartCoroutine(GuestGasPopup(warningPopup));
	}

	private IEnumerator GuestGasPopup(GameObject warningPopup)
	{
		GuestGasWarning gasWarning = warningPopup.GetComponentInChildren<GuestGasWarning>();
		MainMenu menu = Object.FindObjectOfType(typeof(MainMenu)) as MainMenu;
		if (gasWarning != null)
		{
			string returnStatus = null;
			while (gasWarning != null && gasWarning.returnStatus == string.Empty)
			{
				yield return new WaitForSeconds(0.1f);
			}
			switch (gasWarning.returnStatus)
			{
			case "cancel":
				break;
			case "continue":
				makePurchase(lastPurchaseType);
				break;
			case "upgrade":
				if (LoginManager.Instance != null)
				{
					LoginManager.Instance.OnShowCreateAccountMenu(menu.guiController);
				}
				else
				{
					makePurchase(lastPurchaseType);
				}
				break;
			}
		}
		else
		{
			makePurchase(lastPurchaseType);
		}
	}

	private void makePurchase(string productID)
	{
		buyingGas = true;
		ServiceManager.Instance.PurchaseCurrency(productID, 1, OnPurchaseSuccess, OnPurchaseFailure, OnPurchaseCancel);
		lastPurchaseProductID = productID;
		currentContactOverlay = Object.Instantiate(contactingAppleOverlay) as GameObject;
		unitsBought = 1;
		for (int i = 0; i < buttonArranger.Buttons.Length; i++)
		{
			if (buttonArranger.Buttons[i].IAPProductID == productID)
			{
				purchasedProductIndex = i;
			}
		}
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (buyingGas)
		{
			return;
		}
		if (clickSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(clickSounds[Random.Range(0, clickSounds.Length)], Vector3.zero);
		}
		string @string = PlayerPrefs.GetString("username", Language.Get("GUEST_ACCOUNT_PREFIX"));
		switch (b.name)
		{
		case "backBtn":
			if (base.gameObject.activeInHierarchy)
			{
				ReportPurchase(false);
				OnClose();
			}
			break;
		case "FreeGasButton":
			AdManager.Instance.ShowAd(AdType.storeOfferwall);
			EventTracker.TrackEvent(new OfferwallOpenedSchema(new CurrentGasParameter(ServiceManager.Instance.GetStats().gas)));
			break;
		default:
		{
			IIAPButton component = b.transform.parent.GetComponent<AndroidIAPButton>();
			if (@string.StartsWith(Language.Get("GUEST_ACCOUNT_PREFIX")))
			{
				beginPurchaseProcess(component.IAPProductID);
			}
			else
			{
				makePurchase(component.IAPProductID);
			}
			break;
		}
		}
	}

	private void OnOfferWallFinished()
	{
	}

	private void OnPurchaseSuccess()
	{
		OnClose();
		if (currentContactOverlay != null)
		{
			Object.Destroy(currentContactOverlay);
		}
		GoogleIAB.consumeProduct(lastPurchaseProductID);
		CumulativeStats.Instance.numGascansBought += NumberOfGasCans;
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["I_HAVE_GAS"], (float)CumulativeStats.Instance.numGascansBought / 5f * 100f);
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["IM_FEELING_GASSY"], (float)CumulativeStats.Instance.numGascansBought / 25f * 100f);
		ReportPurchase(true);
		ReportAggregatePurchase();
		if (callingObject != null)
		{
			callingObject.SendMessage("OnRefillSuccess", SendMessageOptions.DontRequireReceiver);
			callingObject.SendMessage("GasPurchased", NumberOfGasCans, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnPurchaseFailure()
	{
		if (currentContactOverlay != null)
		{
			Object.Destroy(currentContactOverlay);
		}
		StartCoroutine(showErrorPopup());
	}

	private void OnPurchaseCancel()
	{
		if (currentContactOverlay != null)
		{
			Object.Destroy(currentContactOverlay);
		}
		StartCoroutine(showCancelPopup());
	}

	private IEnumerator showCancelPopup()
	{
		currentCancelOverlay = Object.Instantiate(cancelOverlay) as GameObject;
		yield return new WaitForSeconds(2f);
		buyingGas = false;
		if (currentCancelOverlay != null)
		{
			Object.Destroy(currentCancelOverlay);
		}
	}

	private IEnumerator showErrorPopup()
	{
		currentFailedOverlay = Object.Instantiate(contactingFailedOverlay) as GameObject;
		yield return new WaitForSeconds(2f);
		buyingGas = false;
		if (currentFailedOverlay != null)
		{
			Object.Destroy(currentFailedOverlay);
		}
	}

	private void OnDestroy()
	{
		buyingGas = false;
		if (currentContactOverlay != null)
		{
			Object.Destroy(currentContactOverlay);
		}
		if (currentFailedOverlay != null)
		{
			Object.Destroy(currentFailedOverlay);
		}
		if (ClosingCallback != null)
		{
			ClosingCallback();
		}
	}

	protected override void OnClose()
	{
		EventTracker.TrackEvent(new GetGasClosedSchema());
		base.OnClose();
	}
}
