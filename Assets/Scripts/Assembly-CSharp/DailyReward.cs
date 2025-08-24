using UnityEngine;

public class DailyReward : Popup
{
	public TextMesh mainRewardText;

	public Transform[] rewardDays;

	private string _currencyJoules;

	private string _currencyGas;

	protected override void Start()
	{
		base.Start();
		UpdateLocalizedText();
		int dailyReward = ServiceManager.Instance.GetDailyReward();
		SetRewardDay(dailyReward);
	}

	private void UpdateLocalizedText()
	{
		_currencyJoules = Language.Get("CURRENCY_JOULES");
		_currencyGas = Language.Get("CURRENCY_GAS");
	}

	private void SetRewardDay(int day)
	{
		for (int i = 0; i < rewardDays.Length; i++)
		{
			Reward reward = ServiceManager.Instance.GetReward("daily_reward_" + (i + 1));
			if (reward != null)
			{
				if (day != -1)
				{
					if (i + 1 == day)
					{
						if (reward.gas > 0)
						{
							mainRewardText.text = string.Format("{0:#,0} {1}!", reward.gas, _currencyGas);
						}
						else
						{
							mainRewardText.text = string.Format("{0:#,0} {1}!", reward.joules, _currencyJoules);
						}
					}
					if (i < day)
					{
						rewardDays[i].Find("highlight").gameObject.SetActive(true);
					}
					else
					{
						rewardDays[i].Find("highlight").gameObject.SetActive(false);
					}
				}
				if (reward.gas > 0)
				{
					rewardDays[i].Find("joules_icon").gameObject.SetActive(false);
					rewardDays[i].Find("reward_amt").GetComponent<TextMesh>().text = string.Format("{0:#,0}", reward.gas);
					rewardDays[i].Find("reward_type").GetComponent<TextMesh>().text = _currencyGas;
				}
				else if (reward.joules > 0)
				{
					rewardDays[i].Find("gas_icon").gameObject.SetActive(false);
					rewardDays[i].Find("reward_amt").GetComponent<TextMesh>().text = string.Format("{0:#,0}", reward.joules);
					rewardDays[i].Find("reward_type").GetComponent<TextMesh>().text = _currencyJoules;
				}
			}
			else
			{
				Debug.LogWarning("couldn't get reward with name: daily_reward_" + (i + 1));
			}
		}
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		switch (b.name)
		{
		case "backBtn":
			OnClose();
			break;
		}
	}
}
