using System;
using UnityEngine;

public class SetupPopup
{
	public GameObject PopupPrefab { get; set; }

	public Action<GameObject> SetupMethod { get; set; }

	public SetupPopup(GameObject prefab, Action<GameObject> setupMethod = null)
	{
		PopupPrefab = prefab;
		SetupMethod = setupMethod;
	}
}
