using UnityEngine;

public abstract class SpecialItem : MonoBehaviour
{
	protected string specialIconPath = string.Empty;

	protected PlayerController playerController;

	protected Transform myTransform;

	protected bool isRemote;

	public bool requiresGrounded;

	public float cooldown = 10f;

	public virtual string IconTextureLocation
	{
		get
		{
			return string.Empty;
		}
	}

	public PlayerController PlayerController
	{
		get
		{
			return playerController;
		}
	}

	protected string EquipmentNames
	{
		get
		{
			if (playerController != null && playerController.CharacterManager != null)
			{
				return playerController.CharacterManager.EquipmentNames;
			}
			return string.Empty;
		}
	}

	protected virtual void Awake()
	{
		myTransform = base.transform;
	}

	protected virtual void Start()
	{
	}

	public void Activate(PlayerController p, bool isRemote, float delay)
	{
		playerController = p;
		if (base.name.Contains("(Clone)"))
		{
			base.name = base.name.Remove(base.name.Length - 7);
		}
		if (ServiceManager.Instance != null)
		{
			Configure(ServiceManager.Instance.GetItemByName(base.name));
		}
		OnActivate(p, isRemote, delay);
	}

	protected virtual void Configure(Item item)
	{
		item.UpdateProperty("cooldown", ref cooldown, EquipmentNames);
	}

	protected virtual void OnActivate(PlayerController p, bool isRemote, float delay)
	{
		playerController = p;
		if (!isRemote && playerController.NetSync != null)
		{
			playerController.NetSync.SetAction(15, null);
		}
	}

	public virtual void OnDeactivate(float delay)
	{
		if (!isRemote && playerController.NetSync != null)
		{
			playerController.NetSync.SetAction(16, null);
		}
	}

	protected void OnBombDeactivate()
	{
		base.enabled = false;
		OnDeactivate(0f);
	}
}
