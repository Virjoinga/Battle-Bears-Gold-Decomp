using System.Collections.Generic;

public class Character
{
	public Item characterData;

	public List<Item> skins = new List<Item>();

	public List<Item> taunts = new List<Item>();

	public List<Item> primaryWeapons = new List<Item>();

	public List<Item> secondaryWeapons = new List<Item>();

	public List<Item> meleeWeapons = new List<Item>();

	public Character(Item data)
	{
		characterData = data;
	}
}
