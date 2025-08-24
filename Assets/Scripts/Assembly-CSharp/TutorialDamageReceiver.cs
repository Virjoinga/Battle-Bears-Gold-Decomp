using System.Collections;
using UnityEngine;

public class TutorialDamageReceiver : DamageReceiver
{
	private bool isDying;

	public bool onlyMelee;

	public override void OnTakeDamage(float dmg, int shooterID, bool isExplosion, bool isMelee, bool isHeadshot, bool sendNotification, bool endOfGameOverride = false, float radiationDmg = 0f, string customDeathSfx = "")
	{
		base.OnTakeDamage(dmg, shooterID, isExplosion, isMelee, isHeadshot, sendNotification, endOfGameOverride, radiationDmg, string.Empty);
		if (!isDying)
		{
			if (!onlyMelee)
			{
				StartCoroutine(startDeath());
			}
			else if (isMelee)
			{
				StartCoroutine(startDeathByMelee());
			}
		}
	}

	private IEnumerator startDeath()
	{
		isDying = true;
		base.GetComponent<Animation>().Play("tutorialDeath");
		yield return new WaitForSeconds(base.GetComponent<Animation>()["tutorialDeath"].length);
		Object.Destroy(this);
	}

	private IEnumerator startDeathByMelee()
	{
		isDying = true;
		base.GetComponent<Animation>().Play("hit");
		yield return new WaitForSeconds(base.GetComponent<Animation>()["hit"].length);
		base.GetComponent<Renderer>().material.color = Color.red;
		Object.Destroy(this);
	}
}
