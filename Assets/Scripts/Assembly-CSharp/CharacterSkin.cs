using System;
using UnityEngine;

[Serializable]
public class CharacterSkin : CharacterMod
{
	[SerializeField]
	private Material _material;

	public Material Material
	{
		get
		{
			return _material;
		}
		set
		{
			_material = value;
		}
	}

	public void AddTo(CharacterHandle character)
	{
		character.Skin = _material;
	}

	public void RemoveFrom(CharacterHandle character)
	{
		character.Skin = null;
	}
}
