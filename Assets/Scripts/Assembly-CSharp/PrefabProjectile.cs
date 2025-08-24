using UnityEngine;

public class PrefabProjectile : DamageSource
{
	protected string EquipmentNames
	{
		get
		{
			return equipmentNames;
		}
	}

	public override void OnCollisionEnter(Collision c)
	{
	}

	public override void OnTriggerEnter(Collider collider)
	{
		DamageReceiver damageReceiver = collider.gameObject.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
		if (damageReceiver != null)
		{
			PlayerController component = damageReceiver.transform.root.GetComponent<PlayerController>();
			damageReceiver.OnTakeDamage(damage * base.DamageMultiplier, base.OwnerID, isExplosion, isMelee, false, true, false, 0f, string.Empty);
			if (component != null && component.Team != GameManager.Instance.Players(base.OwnerID).PlayerController.Team)
			{
				Object.Destroy(base.gameObject.transform.parent.gameObject);
			}
		}
	}
}
