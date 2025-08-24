using UnityEngine;

public class ToxicPlague : ProjectileWeapon
{
	[SerializeField]
	private GameObject _constantEffectWhenNotReloading;

	public override void StartedReloading()
	{
		base.StartedReloading();
		_constantEffectWhenNotReloading.SetActive(false);
	}

	public override void OnReload()
	{
		base.OnReload();
		_constantEffectWhenNotReloading.SetActive(true);
	}
}
