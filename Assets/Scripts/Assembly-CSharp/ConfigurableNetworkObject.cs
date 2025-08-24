public class ConfigurableNetworkObject : NetworkObject
{
	protected string equipmentNames = string.Empty;

	protected string configureItemName = string.Empty;

	protected virtual void Start()
	{
		ConfigurableNetworkObject[] componentsInChildren = base.gameObject.GetComponentsInChildren<ConfigurableNetworkObject>();
		ConfigurableNetworkObject[] array = componentsInChildren;
		foreach (ConfigurableNetworkObject configurableNetworkObject in array)
		{
			ForwardSettings(configurableNetworkObject);
			configurableNetworkObject.OwnerID = base.OwnerID;
		}
	}

	public void SetItemOverride(string str)
	{
		configureItemName = str;
	}

	public void SetEquipmentNames(string str)
	{
		equipmentNames = str;
	}

	public void ForwardSettings(ConfigurableNetworkObject n)
	{
		n.SetItemOverride(configureItemName);
		n.SetEquipmentNames(equipmentNames);
	}
}
