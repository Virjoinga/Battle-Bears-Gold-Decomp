using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class Store : MonoBehaviour
{
	public Dictionary<string, Item> items_by_name = new Dictionary<string, Item>();

	public Dictionary<string, Character> characters = new Dictionary<string, Character>();

	public List<Item> specials = new List<Item>();

	public List<Item> equipment = new List<Item>();

	public List<Item> proMode = new List<Item>();

	private static Store instance;

	private Dictionary<string, GoogleSkuInfo> storeProducts = new Dictionary<string, GoogleSkuInfo>();

	public static Store Instance
	{
		get
		{
			return instance;
		}
	}

	public Dictionary<string, GoogleSkuInfo> StoreProducts
	{
		get
		{
			return storeProducts;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void OnGotFreeGas()
	{
		MainMenu mainMenu = Object.FindObjectOfType(typeof(MainMenu)) as MainMenu;
		if (mainMenu != null)
		{
			mainMenu.OnUpdatePlayerStats();
		}
	}

	private void OnFailedToGetFreeGas()
	{
		Debug.LogError("failed to get free gas due to error: " + ServiceManager.Instance.LastError);
	}

	public void addStoreProduct(GoogleSkuInfo g)
	{
		if (!storeProducts.ContainsKey(g.productId))
		{
			storeProducts.Add(g.productId, g);
		}
	}

	public GoogleSkuInfo getStoreProduct(string productID)
	{
		if (storeProducts.ContainsKey(productID))
		{
			return storeProducts[productID];
		}
		return null;
	}

	public void ParseStoreInventory()
	{
		characters.Clear();
		specials.Clear();
		equipment.Clear();
		proMode.Clear();
		items_by_name = ServiceManager.Instance.GetAllItemsByName();
		foreach (KeyValuePair<string, Item> item in items_by_name)
		{
			if (item.Value.type == "character")
			{
				Character character = new Character(item.Value);
				characters.Add(character.characterData.name, character);
			}
			else if (item.Value.type == "special")
			{
				specials.Add(item.Value);
			}
			else if (item.Value.type == "equipment")
			{
				equipment.Add(item.Value);
			}
			else if (item.Value.type == "proMode")
			{
				proMode.Add(item.Value);
			}
		}
		foreach (KeyValuePair<string, Item> item2 in items_by_name)
		{
			if (!(item2.Value.type != "character") || !(item2.Value.type != "special") || !(item2.Value.type != "equipment") || !(item2.Value.type != "pickup") || !(item2.Value.type != "proMode"))
			{
				continue;
			}
			int parent_id = item2.Value.parent_id;
			Character character2 = null;
			foreach (KeyValuePair<string, Character> character3 in characters)
			{
				if (parent_id == character3.Value.characterData.id)
				{
					character2 = character3.Value;
					break;
				}
			}
			if (character2 != null)
			{
				if (item2.Value.type == "skin")
				{
					character2.skins.Add(item2.Value);
				}
				else if (item2.Value.type == "taunt")
				{
					character2.taunts.Add(item2.Value);
				}
				else if (item2.Value.type == "primary")
				{
					character2.primaryWeapons.Add(item2.Value);
				}
				else if (item2.Value.type == "secondary")
				{
					character2.secondaryWeapons.Add(item2.Value);
				}
				else if (item2.Value.type == "melee")
				{
					character2.meleeWeapons.Add(item2.Value);
				}
			}
		}
	}
}
