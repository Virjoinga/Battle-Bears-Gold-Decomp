using UnityEngine;

public class MeleeWeaponDamageArea : MonoBehaviour
{
	private MeleeWeapon ourWeaponRoot;

	private Collider myCollider;

	private void Awake()
	{
		myCollider = base.GetComponent<Collider>();
		ourWeaponRoot = base.transform.root.GetComponent(typeof(MeleeWeapon)) as MeleeWeapon;
	}

	public void OnTriggerEnter(Collider c)
	{
		Physics.IgnoreCollision(c, myCollider);
		if (ourWeaponRoot != null)
		{
			ourWeaponRoot.OnDealDamageFromSubObject(c.gameObject, myCollider);
		}
	}
}
