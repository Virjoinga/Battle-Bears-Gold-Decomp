public class RadiationExplosionDamageSource : InstantaneousDamageSource
{
	public float maxDamageFromHealth = 0.5f;

	public float minDamageFromHealth = 0.25f;

	public float maxRangeFromHealth = 0.5f;

	public float minRangeFromHealth = 1f;

	public float radiationDamageFloor = 25f;

	public float radiationDamageCeiling = 100f;

	protected override void Start()
	{
		ServiceManager.Instance.UpdateProperty("rad_min_damage_mod", ref minDamageFromHealth);
		ServiceManager.Instance.UpdateProperty("rad_max_damage_mod", ref maxDamageFromHealth);
		ServiceManager.Instance.UpdateProperty("rad_min_range_mod", ref minRangeFromHealth);
		ServiceManager.Instance.UpdateProperty("rad_max_range_mod", ref maxRangeFromHealth);
		ServiceManager.Instance.UpdateProperty("rad_damage_ceil", ref radiationDamageCeiling);
		ServiceManager.Instance.UpdateProperty("rad_damage_floor", ref radiationDamageFloor);
		StartCoroutine(delayedSelfDestruct());
	}

	public void SetValuesFromHealth(float health)
	{
		minDamage = health * minDamageFromHealth;
		maxDamage = health * maxDamageFromHealth;
		if (minDamage > radiationDamageCeiling)
		{
			minDamage = radiationDamageCeiling;
		}
		if (maxDamage > radiationDamageCeiling)
		{
			maxDamage = radiationDamageCeiling;
		}
		if (minDamage < radiationDamageFloor)
		{
			minDamage = radiationDamageFloor;
		}
		if (maxDamage < radiationDamageFloor)
		{
			maxDamage = radiationDamageFloor;
		}
		minDamageDistance = health * minRangeFromHealth;
		maxDamageDistance = health * maxRangeFromHealth;
	}
}
