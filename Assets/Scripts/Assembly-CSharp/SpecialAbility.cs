using System.Collections;
using UnityEngine;

public abstract class SpecialAbility : MonoBehaviour
{
	protected PlayerController playerController;

	public float duration;

	public GameObject effectPrefab;

	public float damage;

	protected virtual void Awake()
	{
		playerController = GetComponent(typeof(PlayerController)) as PlayerController;
	}

	protected virtual void Start()
	{
		StartCoroutine(delayedDeactivate());
	}

	protected IEnumerator delayedDeactivate()
	{
		yield return new WaitForSeconds(duration);
		base.enabled = false;
	}

	public virtual void OnDeactivateSpecialAbility()
	{
		if (playerController != null)
		{
			playerController = GetComponent(typeof(PlayerController)) as PlayerController;
		}
		base.enabled = false;
		if (playerController != null && !playerController.isRemote && playerController.NetSync != null)
		{
			playerController.NetSync.SetAction(17, null);
		}
	}

	public void OnBombDeactivate()
	{
		OnDeactivateSpecialAbility();
	}
}
