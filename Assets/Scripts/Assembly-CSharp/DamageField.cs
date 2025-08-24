using UnityEngine;

public class DamageField : EffectField
{
	[SerializeField]
	private float _damageTakenPercent = 1.5f;

	protected override void ApplyEffect(PlayerController pc)
	{
		base.ApplyEffect(pc);
		pc.DamageReceiver.DamageMultipliers.Add(_damageTakenPercent);
	}

	protected override void RemoveEffect(PlayerController pc)
	{
		base.RemoveEffect(pc);
		pc.DamageReceiver.DamageMultipliers.Remove(_damageTakenPercent);
	}
}
