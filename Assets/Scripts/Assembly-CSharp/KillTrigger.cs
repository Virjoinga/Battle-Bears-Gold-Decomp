using System.Collections;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
	public Animation animationAction;

	public string animationToPlay;

	public string animationDefault;

	public KillTrigger[] killColliders;

	public bool ActivateKill;

	public float timeToStart = 3f;

	private bool isAnimating;

	public void OnTriggerEnter(Collider c)
	{
		dealDamage(c.gameObject);
	}

	public void OnCollisionEnter(Collision c)
	{
		dealDamage(c.gameObject);
	}

	public void OnCollisionStay(Collision c)
	{
		dealDamage(c.gameObject);
	}

	private void dealDamage(GameObject target)
	{
		if (ActivateKill)
		{
			DamageReceiver damageReceiver = target.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
			if (damageReceiver != null)
			{
				damageReceiver.OnKilledByDeathArea();
			}
		}
		else if (!isAnimating)
		{
			StartCoroutine(delayAnimation());
		}
	}

	private IEnumerator delayAnimation()
	{
		isAnimating = true;
		yield return new WaitForSeconds(timeToStart);
		if (animationAction != null)
		{
			animationAction.Play(animationToPlay);
			yield return new WaitForSeconds(animationAction.clip.length + 2f);
			animationAction.Play(animationDefault);
		}
		isAnimating = false;
	}

	private void ActivateKillCollider()
	{
		ActivateKill = true;
		KillTrigger[] array = killColliders;
		foreach (KillTrigger killTrigger in array)
		{
			killTrigger.ActivateKill = true;
		}
	}

	private void DeactivateKillCollider()
	{
		ActivateKill = false;
		KillTrigger[] array = killColliders;
		foreach (KillTrigger killTrigger in array)
		{
			killTrigger.ActivateKill = false;
		}
	}
}
