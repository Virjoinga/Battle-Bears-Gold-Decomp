using System.Collections;
using UnityEngine;

public class Poison : MonoBehaviour
{
	public float poisonAmount;

	public float poisonDuration = 10f;

	public int poisonerPlayerID = -1;

	public bool showPoisonIcon = true;

	public string customDeathSfx = string.Empty;

	private PlayerController playerAffected;

	private void Start()
	{
		StartCoroutine(poisonEffect(poisonAmount, poisonDuration, poisonerPlayerID));
	}

	private IEnumerator poisonEffect(float amount, float duration, int poisonerID)
	{
		playerAffected = GetComponent<PlayerController>();
		if (playerAffected != null)
		{
			if (showPoisonIcon)
			{
				playerAffected.OnGetPoisoned();
			}
			float poisonInterval = 0.15f;
			while (duration > 0f && GameManager.Instance.TimeLeft > 2 && !GameManager.Instance.IsGameSubmitted && playerAffected.DamageReceiver.CurrentHP > 0f)
			{
				playerAffected.DamageReceiver.OnTakeDamage(poisonAmount * poisonInterval, poisonerID, false, false, false, true, false, 0f, customDeathSfx);
				yield return new WaitForSeconds(poisonInterval);
				duration -= poisonInterval;
			}
		}
		Object.Destroy(this);
	}

	private void OnDestroy()
	{
		if (playerAffected != null)
		{
			playerAffected.OnRemovePoison();
		}
	}
}
