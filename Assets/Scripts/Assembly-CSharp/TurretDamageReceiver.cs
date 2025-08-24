using ExitGames.Client.Photon;
using UnityEngine;

public class TurretDamageReceiver : DamageReceiver
{
	public string itemName = string.Empty;

	public float startHealth = 100f;

	public bool isRemote;

	public int weaponIndex = -1;

	public int turretIndex = -1;

	private PlayerController _owningPlayer;

	private int _timeMod;

	private int _currentHealth = 100000;

	private NetSyncReporter _netSyncReporter;

	private DeployableTurret _thisTurret;

	protected string equipmentNames = string.Empty;

	protected string configureItemName = string.Empty;

	public PlayerController OwningPlayer
	{
		get
		{
			return _owningPlayer;
		}
		set
		{
			_owningPlayer = value;
			_netSyncReporter = value.NetSync;
		}
	}

	public float CurrentHP
	{
		get
		{
			return (float)(_currentHealth ^ _timeMod) / 1000f;
		}
		set
		{
			_timeMod = (int)(Time.time * 1000f);
			_currentHealth = (int)(value * 1000f) ^ _timeMod;
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

	public void Start()
	{
		CurrentHP = startHealth;
		_thisTurret = base.gameObject.GetComponent<DeployableTurret>();
		ConfigureObject();
	}

	public void ConfigureObject()
	{
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("health", ref startHealth, equipmentNames);
		}
	}

	public bool atMaxHealth()
	{
		return CurrentHP >= startHealth;
	}

	public bool atDoubleMaxHealth()
	{
		return CurrentHP >= startHealth * 2f;
	}

	public override void OnTakeDamage(float dmg, int shooterID, bool isExplosion, bool isMelee, bool isHeadshot, bool sendNotification, bool endOfGameOverride = false, float radiationDmg = 0f, string customDeathSfx = "")
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		base.OnTakeDamage(dmg, shooterID, isExplosion, isMelee, isHeadshot, sendNotification, endOfGameOverride, radiationDmg, string.Empty);
		if (CurrentHP <= 0f || (isInvincible && !endOfGameOverride))
		{
			return;
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(shooterID);
		PlayerCharacterManager playerCharacterManager2 = GameManager.Instance.Players(base.OwnerID);
		if (!deathWasFromDeathArea && GameManager.Instance.friendlyFireRatio < 0.01f && playerCharacterManager != null && playerCharacterManager2 != null && playerCharacterManager.team == playerCharacterManager2.team && shooterID != base.OwnerID)
		{
			return;
		}
		if (isRemote)
		{
			if (!isShowingHit && base.gameObject.activeInHierarchy)
			{
				StartCoroutine(displayHit(dmg));
			}
			return;
		}
		if (!deathWasFromDeathArea && playerCharacterManager != null && playerCharacterManager2 != null && playerCharacterManager.team == playerCharacterManager2.team && shooterID != base.OwnerID)
		{
			dmg *= GameManager.Instance.friendlyFireRatio;
		}
		CurrentHP -= dmg;
		if (sendNotification && _netSyncReporter != null)
		{
			if (playerCharacterManager != null)
			{
				GameManager.Instance.playerStats[shooterID].addDamageDealt(base.OwnerID, dmg);
			}
			StopCoroutine("shieldCountdown");
			Hashtable hashtable = new Hashtable();
			hashtable[(byte)0] = base.OwnerID;
			hashtable[(byte)1] = dmg;
			hashtable[(byte)2] = shooterID;
			_netSyncReporter.SetAction(35, hashtable);
		}
		if (CurrentHP <= 0f)
		{
			isInvincible = true;
			if (shooterID == -1)
			{
				shooterID = base.OwnerID;
			}
			_thisTurret.OnDetonateDeployable(OwningPlayer, false);
		}
		else
		{
			if (!isShowingHit)
			{
				StartCoroutine(displayHit(dmg));
			}
			SendMessageUpwards("OnHealth", CurrentHP / startHealth, SendMessageOptions.DontRequireReceiver);
		}
	}
}
