using System.Collections;
using UnityEngine;

public class HealOverTime : MonoBehaviour
{
	public float healAmount = 5f;

	public float tickSpeed = 1f;

	public float duration = 15f;

	private PlayerController _playerToHeal;

	private void Start()
	{
		_playerToHeal = GetComponent<PlayerController>();
		if (_playerToHeal == null)
		{
			Object.Destroy(this);
		}
		StartCoroutine(Heal());
	}

	private IEnumerator Heal()
	{
		float startTime = Time.time;
		while (Time.time - startTime < duration)
		{
			if (_playerToHeal != null)
			{
				_playerToHeal.DamageReceiver.addHealth(healAmount);
			}
			yield return new WaitForSeconds(tickSpeed);
		}
	}
}
