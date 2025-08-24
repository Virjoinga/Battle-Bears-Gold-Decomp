using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialOffersManager : MonoBehaviour
{
	public ImageAnimator[] adPlateAnimators;

	public List<GUIButton> specialOfferButtons = new List<GUIButton>();

	public AnimatedScroller specialOffersScroller;

	public GameObject[] rewardPrefabs;

	public string[] rewardNames;

	public GameObject dealPrefab;

	private void Start()
	{
		updateSpecialOffers();
	}

	public void updateSpecialOffers()
	{
		resetSpecialOfferButtons();
		List<int> previouslyReceivedRewards = updateSpecialOffersNotDeals();
		showDeals();
		updatePreviouslyRecievedOffers(previouslyReceivedRewards);
	}

	private void resetSpecialOfferButtons()
	{
		foreach (GUIButton specialOfferButton in specialOfferButtons)
		{
			Object.Destroy(specialOfferButton.gameObject);
		}
		specialOfferButtons.Clear();
		specialOffersScroller.OnReset();
	}

	private void showDeals()
	{
		int val = -1;
		ServiceManager.Instance.UpdateProperty("gold_skins_deal", ref val);
		foreach (Deal deal in ServiceManager.Instance.GetDeals())
		{
			bool flag = true;
			foreach (int item_id in deal.item_ids)
			{
				if (!ServiceManager.Instance.IsItemBought(item_id))
				{
					flag = false;
					break;
				}
			}
			if (!flag && deal.id != val)
			{
				GameObject gameObject = Object.Instantiate(dealPrefab) as GameObject;
				gameObject.name = "deal_" + deal.id;
				GUIButton componentInChildren = gameObject.GetComponentInChildren<GUIButton>();
				specialOffersScroller.addButton(componentInChildren);
				specialOfferButtons.Add(componentInChildren);
				StartCoroutine(grabDealImage(gameObject.transform.Find("icon").GetComponentInChildren<Renderer>(), deal.button_image_url));
			}
		}
	}

	private IEnumerator grabDealImage(Renderer r, string url)
	{
		yield return new WaitForSeconds(10f);
		if (!(r == null))
		{
			WWW www = new WWW(url);
			yield return www;
			if (www.error != null)
			{
				Debug.LogWarning("WWW download error: " + www.error);
				yield break;
			}
			r.material.mainTexture = www.texture;
			r.material.color = Color.grey;
			r.transform.parent.Find("loadText").gameObject.SetActive(false);
		}
	}

	public List<int> updateSpecialOffersNotDeals()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < rewardNames.Length; i++)
		{
			Reward reward = ServiceManager.Instance.GetReward(rewardNames[i]);
			if (reward != null)
			{
				if (ServiceManager.Instance.PlayerHasReward(rewardNames[i]))
				{
					list.Add(i);
				}
				else
				{
					addSpecialOffer(rewardPrefabs[i], reward, rewardNames[i], false);
				}
				continue;
			}
			int val = -1;
			ServiceManager.Instance.UpdateProperty("offerwall_level", ref val);
			if (val != -1 && ServiceManager.Instance.GetStats().level >= (double)val)
			{
				addSpecialOffer(rewardPrefabs[i], null, rewardNames[i], false);
			}
		}
		return list;
	}

	public void updatePreviouslyRecievedOffers(List<int> previouslyReceivedRewards)
	{
		foreach (int previouslyReceivedReward in previouslyReceivedRewards)
		{
			Reward reward = ServiceManager.Instance.GetReward(rewardNames[previouslyReceivedReward]);
			if (reward != null)
			{
				addSpecialOffer(rewardPrefabs[previouslyReceivedReward], reward, rewardNames[previouslyReceivedReward], true);
			}
		}
	}

	private void addSpecialOffer(GameObject rewardPrefab, Reward reward, string rewardName, bool alreadyReceivedReward)
	{
		GameObject gameObject = Object.Instantiate(rewardPrefab) as GameObject;
		if (!(gameObject != null))
		{
			return;
		}
		gameObject.name = rewardName;
		GUIButton gUIButton = gameObject.GetComponent(typeof(GUIButton)) as GUIButton;
		if (reward != null)
		{
			if (alreadyReceivedReward)
			{
				(gameObject.transform.Find("offer_value").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Format("{0:#,0}", 0);
				gUIButton.greyify();
			}
			else
			{
				(gameObject.transform.Find("offer_value").GetComponent(typeof(TextMesh)) as TextMesh).text = string.Format("{0:#,0}", reward.gas);
			}
		}
		specialOffersScroller.addButton(gUIButton);
		specialOfferButtons.Add(gUIButton);
	}
}
