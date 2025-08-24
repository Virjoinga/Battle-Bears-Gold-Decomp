using System.Collections;
using System.Collections.Generic;
using Analytics;
using Analytics.Schemas;
using Prime31;
using UnityEngine;

public class BuyJoules : Popup
{
	private bool buyingJoules;

	public GameObject contactingAppleOverlay;

	public GameObject contactingFailedOverlay;

	public GameObject cancelOverlay;

	private GameObject currentContactOverlay;

	private GameObject currentFailedOverlay;

	private GameObject currentCancelOverlay;

	public IAPButtonArranger buttonArranger;

	private int unitsBought;

	private int purchasedProductIndex;

	private string lastPurchaseType;

	public AudioClip[] clickSounds;

	public TextMesh currentGasDisplay;

	public TextMesh currentJoulesDisplay;

	public static string lastPurchaseProductID = string.Empty;

	public string ValueOfPurchase
	{
		get
		{
			return buttonArranger.Buttons[purchasedProductIndex].RealMoneyPrice;
		}
	}

	public int NumberOfJoules
	{
		get
		{
			if (buttonArranger.Buttons[purchasedProductIndex].CurrencyType == IAPButtonCurrencyTypes.Joules)
			{
				return unitsBought * buttonArranger.Buttons[purchasedProductIndex].InGameCurrencyValue;
			}
			return 0;
		}
	}

	protected override void Start()
	{
		currentGasDisplay.text = string.Format("{0:#,0}", ServiceManager.Instance.GetStats().gas);
		currentJoulesDisplay.text = string.Format("{0:#,0}", ServiceManager.Instance.GetStats().joules);
		SetupButtonArranger();
		EventTracker.TrackEvent(new GetJoulesOpenedSchema());
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
		List<KeyValuePair<string, int>> joulesProductsSorted = GetJoulesProductsSorted(googlePlayProducts);
		int num = 0;
		foreach (KeyValuePair<string, int> item in joulesProductsSorted)
		{
			buttonArranger.Buttons[num].IAPProductID = item.Key;
			buttonArranger.Buttons[num].CurrencyType = ((!item.Key.Contains("gascan")) ? IAPButtonCurrencyTypes.Joules : IAPButtonCurrencyTypes.GasCans);
			buttonArranger.Buttons[num].InGameCurrencyValue = item.Value;
			GoogleSkuInfo storeProduct = Store.Instance.getStoreProduct(item.Key);
			if (storeProduct != null)
			{
				buttonArranger.Buttons[num].RealMoneyPrice = storeProduct.price;
			}
			num++;
		}
		for (int i = num; i < buttonArranger.Buttons.Length; i++)
		{
			buttonArranger.Buttons[i].Disabled = true;
		}
	}

	private List<KeyValuePair<string, int>> GetJoulesProductsSorted(Dictionary<string, int> productIds)
	{
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
		if (productIds != null)
		{
			foreach (KeyValuePair<string, int> productId in productIds)
			{
				if (productId.Key.Contains("joules"))
				{
					list.Add(productId);
				}
			}
			list.Sort((KeyValuePair<string, int> x, KeyValuePair<string, int> y) => x.Value.CompareTo(y.Value));
		}
		return list;
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
			mainMenu.TryCreatePopup(new SetupPopup(mainMenu.guestGasWarningPopup, SetupGasWarningPopup));
		}
		else
		{
			makePurchase(purchaseType);
		}
	}

	private void SetupGasWarningPopup(GameObject warningPopup)
	{
		StartCoroutine(GasWarningPopup(warningPopup));
	}

	private IEnumerator GasWarningPopup(GameObject warningPopup)
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
		menu.currentPopup = base.gameObject;
	}

	private void makePurchase(string productID)
	{
		buyingJoules = true;
		ServiceManager.Instance.PurchaseCurrency(productID, 1, OnPurchaseSuccess, OnPurchaseFailure, OnPurchaseCancel);
		lastPurchaseProductID = productID;
		currentContactOverlay = Object.Instantiate(contactingAppleOverlay) as GameObject;
		unitsBought = 1;
		purchasedProductIndex = 0;
	}

	private void OnPurchaseSuccess()
	{
		OnClose();
		if (currentContactOverlay != null)
		{
			Object.Destroy(currentContactOverlay);
		}
		GoogleIAB.consumeProduct(lastPurchaseProductID);
		CumulativeStats.Instance.numJoulesBought += NumberOfJoules;
		ReportPurchase(true);
		ReportAggregatePurchase();
		if (callingObject != null)
		{
			callingObject.SendMessage("OnJoulesSuccess", SendMessageOptions.DontRequireReceiver);
			callingObject.SendMessage("JoulesPurchased", NumberOfJoules, SendMessageOptions.DontRequireReceiver);
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
		buyingJoules = false;
		if (currentCancelOverlay != null)
		{
			Object.Destroy(currentCancelOverlay);
		}
	}

	private IEnumerator showErrorPopup()
	{
		currentFailedOverlay = Object.Instantiate(contactingFailedOverlay) as GameObject;
		yield return new WaitForSeconds(2f);
		buyingJoules = false;
		if (currentFailedOverlay != null)
		{
			Object.Destroy(currentFailedOverlay);
		}
	}

	protected override void OnClose()
	{
		EventTracker.TrackEvent(new GetJoulesClosedSchema());
		base.OnClose();
	}

	private void OnDestroy()
	{
		buyingJoules = false;
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

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (buyingJoules)
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
		default:
		{
			IIAPButton component = b.transform.GetComponent<AndroidIAPButton>();
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
}
