using System;
using UnityEngine;

public class InsufficientFunds : Popup
{
	public TextMesh amountStillNeeded;

	public TextMesh currentJouleDisplay;

	public TextMesh currentGasDisplay;

	private Stats stats;

	private int costOfItem;

	private MainMenu mainMenu;

	public AudioClip inSound;

	public AudioClip outSound;

	public AudioClip[] clickSounds;

	private bool isGas;

	public GameObject gasIcon;

	public GameObject jouleIcon;

	protected override void Start()
	{
		base.Start();
		if (inSound != null)
		{
			AudioSource.PlayClipAtPoint(inSound, Vector3.zero);
		}
	}

	public void OnSetMainMenu(MainMenu m)
	{
		mainMenu = m;
	}

	public void OnSetItemCost(int cost, bool gas)
	{
		isGas = gas;
		stats = ServiceManager.Instance.GetStats();
		costOfItem = cost;
		updateInfo();
	}

	private void updateInfo()
	{
		currentJouleDisplay.text = string.Format("{0:#,0}", stats.joules);
		currentGasDisplay.text = string.Format("{0:#,0}", stats.gas);
		if (isGas)
		{
			jouleIcon.SetActive(false);
			amountStillNeeded.text = string.Format("{0:#,0}", costOfItem - stats.gas);
		}
		else
		{
			gasIcon.SetActive(false);
			amountStillNeeded.text = string.Format("{0:#,0}", costOfItem - stats.joules);
		}
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (clickSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(clickSounds[UnityEngine.Random.Range(0, clickSounds.Length)], Vector3.zero);
		}
		switch (b.name)
		{
		case "backBtn":
			if (base.gameObject.activeInHierarchy)
			{
				if (outSound != null)
				{
					AudioSource.PlayClipAtPoint(outSound, Vector3.zero);
				}
				OnClose();
				if (callingObject != null)
				{
					callingObject.SendMessage("OnUpdatePlayerStats", SendMessageOptions.DontRequireReceiver);
				}
				if (ClosingCallback != null)
				{
					ClosingCallback();
				}
			}
			break;
		case "buyGasButton":
			mainMenu.OnCreateBuyGasPopup(SetupGasPopup);
			break;
		case "tradeButton":
			mainMenu.OnCreateBuyJoulesPopup(SetupJoulesPopup);
			break;
		}
	}

	private void SetupGasPopup(GameObject gasPopup)
	{
		BuyGas component = gasPopup.GetComponent<BuyGas>();
		component.OnSetCallingObject(callingObject, popupCamera);
		component.ClosingCallback = (FinishedCallback)Delegate.Combine(component.ClosingCallback, new FinishedCallback(FinishedIAPMenu));
		base.gameObject.SetActive(false);
	}

	private void SetupJoulesPopup(GameObject joulesPopup)
	{
		BuyJoules component = joulesPopup.GetComponent<BuyJoules>();
		component.OnSetCallingObject(callingObject, popupCamera);
		component.ClosingCallback = (FinishedCallback)Delegate.Combine(component.ClosingCallback, new FinishedCallback(FinishedIAPMenu));
		base.gameObject.SetActive(false);
	}

	private void FinishedIAPMenu()
	{
		base.gameObject.SetActive(true);
	}
}
