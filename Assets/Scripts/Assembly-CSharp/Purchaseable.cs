public class Purchaseable
{
	public int id = -1;

	public int item_id = -1;

	public int base_joules;

	public int base_gas;

	public int current_joules;

	public int current_gas;

	public bool is_for_sale = true;

	public Item GetItem()
	{
		return ServiceManager.Instance.GetItemByID(item_id);
	}
}
