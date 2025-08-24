using UnityEngine;

public class ItemDescription : Popup
{
	public TextMesh itemTitle;

	public TextBlock itemDescription;

	public Transform iconMount;

	public void OnShowItem(Item item)
	{
		Object @object = null;
		if (!(item.type == "character"))
		{
			@object = ((item.type == "skin") ? Resources.Load("Icons/Characters/" + ServiceManager.Instance.GetItemByID(item.parent_id).name + "/" + item.name + "_red") : ((item.type == "special") ? Resources.Load("Icons/Specials/" + item.name) : ((!(item.type == "equipment")) ? Resources.Load("Icons/Weapons/" + ServiceManager.Instance.GetItemByID(item.parent_id).name + "/" + item.name) : Resources.Load("Icons/Equipment/" + item.name))));
		}
		else
		{
			string empty = string.Empty;
			for (int i = 0; i < Store.Instance.characters[item.name].skins.Count; i++)
			{
				if (Store.Instance.characters[item.name].skins[i].is_default)
				{
					empty = Store.Instance.characters[item.name].skins[i].name;
				}
			}
			@object = Resources.Load("Icons/Characters/" + item.name + "/" + empty + "_red");
		}
		if (@object != null)
		{
			GameObject gameObject = Object.Instantiate(@object) as GameObject;
			gameObject.transform.parent = iconMount;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = Vector3.zero;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.gameObject.layer = LayerMask.NameToLayer("HUD");
		}
		itemTitle.text = item.title;
		itemDescription.OnSetText(item.description, string.Empty);
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (b != null)
		{
			switch (b.name)
			{
			case "backBtn":
				OnClose();
				break;
			}
		}
	}
}
