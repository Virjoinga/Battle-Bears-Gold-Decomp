using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnOwnerDeath : MonoBehaviour
{
	private PlayerController _owner;

	private void Start()
	{
		StartCoroutine(DelayedInit());
	}

	private void Update()
	{
		if (_owner != null && _owner.IsDead)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator DelayedInit()
	{
		yield return null;
		ConfigurableNetworkObject cno = GetComponent<ConfigurableNetworkObject>();
		if (!(cno != null))
		{
			yield break;
		}
		List<PlayerCharacterManager> managers = GameManager.Instance.GetPlayerCharacterManagers();
		foreach (PlayerCharacterManager manager in managers)
		{
			if (cno.OwnerID == manager.OwnerID)
			{
				_owner = manager.PlayerController;
				break;
			}
		}
	}
}
