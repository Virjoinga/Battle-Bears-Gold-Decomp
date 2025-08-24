public class DeployableDamageAndSlowField : DeployableSlowField
{
	protected override void Start()
	{
		base.Start();
		SimpleConstantDamageSource simpleConstantDamageSource = base.gameObject.AddComponent<SimpleConstantDamageSource>();
		simpleConstantDamageSource.OwnerID = base.OwnerID;
		simpleConstantDamageSource.SetItemOverride(itemOverride);
		simpleConstantDamageSource.SetEquipmentNames(equipmentNames);
	}
}
