using System;
using UnityEngine;

public class BuyGoldPack : Popup
{
	public AudioClip[] clickSounds;

	public TextMesh description;

	public TextMesh price;

	[HideInInspector]
	public int gasPurchased;

	[HideInInspector]
	public int joulesPurchased;

	public Action<BuyGoldPack> OnPackPurchaseSuccess;

	public Action<BuyGoldPack> OnPackPurchaseFailure;

	public Action<BuyGoldPack> OnPackPurchaseCancel;

	private int _bundleID;

	private Deal _purchaseInfo;

	private MainMenu _mainMenu;

	protected override void Start()
	{
		ServiceManager.Instance.UpdateProperty("gold_skins_deal", ref _bundleID);
		_purchaseInfo = ServiceManager.Instance.GetDeal(_bundleID);
		GameObject gameObject = GameObject.Find("mainMenu_optimized");
		_mainMenu = gameObject.GetComponent<MainMenu>();
		price.text = string.Format("{0:#,#}", _purchaseInfo.gas);
		description.text = _purchaseInfo.description;
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (clickSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(clickSounds[UnityEngine.Random.Range(0, clickSounds.Length)], Vector3.zero);
		}
		string @string = PlayerPrefs.GetString("username", Language.Get("GUEST_ACCOUNT_PREFIX"));
		switch (b.name)
		{
		case "backBtn":
			if (base.gameObject.activeInHierarchy)
			{
				if (OnPackPurchaseCancel != null)
				{
					OnPackPurchaseCancel(this);
				}
				OnClose();
			}
			break;
		default:
			AttemptGoldSkinsPackPurchase();
			break;
		}
	}

	private void AttemptGoldSkinsPackPurchase()
	{
		int? gas = _purchaseInfo.gas;
		if (gas.HasValue && ServiceManager.Instance.GetStats().gas < gas.Value)
		{
			GameObject gameObject = GameObject.Find("gearup__");
			if (gameObject != null)
			{
				Gearup component = gameObject.GetComponent<Gearup>();
				int? gas2 = _purchaseInfo.gas;
				int num;
				if (gas2.HasValue)
				{
					int? gas3 = _purchaseInfo.gas;
					num = gas3.Value;
				}
				else
				{
					num = 0;
				}
				int num2 = num;
				int? joules = _purchaseInfo.joules;
				int num3;
				if (joules.HasValue)
				{
					int? joules2 = _purchaseInfo.joules;
					num3 = joules2.Value;
				}
				else
				{
					num3 = 0;
				}
				int num4 = num3;
				GameObject gameObject2 = component.createShopPopup(_mainMenu.tradeGasShopPopup, (num2 <= 0) ? num4 : num2, num2 > 0);
				InsufficientFunds component2 = gameObject2.GetComponent<InsufficientFunds>();
				component2.ClosingCallback = InsufficientFundsFinished;
				base.gameObject.SetActive(false);
			}
		}
		else
		{
			ServiceManager.Instance.PurchaseDeal(_bundleID, delegate
			{
				OnPackPurchaseSuccess(this);
			}, delegate
			{
				OnPackPurchaseFailure(this);
			});
			OnClose();
		}
	}

	private void GasPurchased(int gasCans)
	{
		gasPurchased += gasCans;
	}

	private void JoulesPurchased(int joules)
	{
		joulesPurchased += joules;
	}

	private void InsufficientFundsFinished()
	{
		base.gameObject.SetActive(true);
	}
}
