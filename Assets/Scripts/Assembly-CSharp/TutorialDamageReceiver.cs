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
		base.animation.Play("tutorialDeath");
		yield return new WaitForSeconds(base.animation["tutorialDeath"].length);
		Object.Destroy(this);
	}

	private IEnumerator startDeathByMelee()
	{
		isDying = true;
		base.animation.Play("hit");
		yield return new WaitForSeconds(base.animation["hit"].length);
		base.renderer.material.color = Color.red;
		Object.Destroy(this);
	}
}
