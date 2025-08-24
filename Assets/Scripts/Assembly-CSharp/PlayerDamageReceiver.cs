using System.Collections;
using System.Collections.Generic;
using Analytics;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerDamageReceiver : DamageReceiver
{
	public string itemName = string.Empty;

	public float startHealth = 100f;

	private int _currentHealth = 100000;

	private Transform myTransform;

	private PlayerController playerController;

	public bool isRemote;

	private NetSyncReporter netSyncReporter;

	private bool isGrabbingBomb;

	private int _timeMod;

	private List<float> _damageMultipliers = new List<float>();

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

	public List<float> DamageMultipliers
	{
		get
		{
			return _damageMultipliers;
		}
		set
		{
			_damageMultipliers = value;
		}
	}

	public float DamageShieldValue { get; set; }

	public string EquipmentNames
	{
		get
		{
			if (playerController != null && playerController.CharacterManager != null)
			{
				PlayerLoadout playerLoadout = playerController.CharacterManager.playerLoadout;
				if (playerLoadout != null)
				{
					string text = string.Empty;
					if (playerLoadout.equipment1 != null)
					{
						text += playerLoadout.equipment1.name;
					}
					text += "|";
					if (playerLoadout.equipment2 != null)
					{
						text += playerLoadout.equipment2.name;
					}
					return text;
				}
			}
			return string.Empty;
		}
	}

	private void Awake()
	{
		myTransform = base.transform;
		myAudio = base.audio;
		playerController = myTransform.root.GetComponentInChildren<PlayerController>();
		if (playerController.CharacterHandle != null)
		{
			renderers = playerController.CharacterHandle.Renderers;
		}
	}

	private void Start()
	{
		Item itemByName = ServiceManager.Instance.GetItemByName(itemName);
		itemByName.UpdateProperty("health", ref startHealth, EquipmentNames);
		if (playerController.CharacterManager != null)
		{
			float num = 0f;
			Item skin = playerController.CharacterManager.playerLoadout.skin;
			if (skin != null)
			{
				num += skin.GetBonusProperty("healthPercent");
			}
			Item primary = playerController.CharacterManager.playerLoadout.primary;
			num += primary.GetBonusProperty("healthPercent");
			Item secondary = playerController.CharacterManager.playerLoadout.secondary;
			num += secondary.GetBonusProperty("healthPercent");
			Item melee = playerController.CharacterManager.playerLoadout.melee;
			num += melee.GetBonusProperty("healthPercent");
			Item special = playerController.CharacterManager.playerLoadout.special;
			if (special != null)
			{
				num += special.GetBonusProperty("healthPercent");
			}
			Item equipment = playerController.CharacterManager.playerLoadout.equipment1;
			if (equipment != null)
			{
				num += equipment.GetBonusProperty("healthPercent");
			}
			Item equipment2 = playerController.CharacterManager.playerLoadout.equipment2;
			if (equipment2 != null)
			{
				num += equipment2.GetBonusProperty("healthPercent");
			}
			num /= 100f;
			startHealth += startHealth * num;
		}
		CurrentHP = startHealth;
	}

	public void OnPostCreate()
	{
		netSyncReporter = GetComponent(typeof(NetSyncReporter)) as NetSyncReporter;
	}

	public bool atMaxHealth()
	{
		return CurrentHP >= startHealth;
	}

	public bool atDoubleMaxHealth()
	{
		return CurrentHP >= startHealth * 2f;
	}

	public void OnGiveBombGrabInvincibility()
	{
		StartCoroutine(temporaryBombGrabInvincibility());
	}

	public void OnRemoveBombGrabInvincibility()
	{
		isGrabbingBomb = false;
	}

	private IEnumerator temporaryBombGrabInvincibility()
	{
		isGrabbingBomb = true;
		yield return new WaitForSeconds(2f);
		isGrabbingBomb = false;
	}

	private IEnumerator DelayedHealRadiationDamageOverTime()
	{
		yield return new WaitForSeconds(4f);
		while (_radiationDmg > 0f)
		{
			_radiationDmg -= 25f * Time.deltaTime;
			playerController.SetRadiationDisplay(_radiationDmg / startHealth);
			yield return null;
		}
		_radiationDmg = 0f;
	}

	public float addHealthPercentage(float percent)
	{
		float num = startHealth * (percent / 100f);
		float result = num;
		CurrentHP += num;
		if (CurrentHP > startHealth * 2f)
		{
			result = num - (CurrentHP - startHealth * 2f);
			CurrentHP = startHealth * 2f;
		}
		SendMessageUpwards("OnHealth", CurrentHP / startHealth, SendMessageOptions.DontRequireReceiver);
		return result;
	}

	public float addHealth(float amount)
	{
		if (atMaxHealth())
		{
			return 0f;
		}
		float result = amount;
		CurrentHP += amount;
		if (CurrentHP > startHealth)
		{
			result = amount - (CurrentHP - startHealth);
			CurrentHP = startHealth;
		}
		SendMessageUpwards("OnHealth", CurrentHP / startHealth, SendMessageOptions.DontRequireReceiver);
		return result;
	}

	public void OnInvincibility(float duration)
	{
		StopCoroutine("shieldCountdown");
		StartCoroutine("shieldCountdown", duration);
	}

	private IEnumerator shieldCountdown(float duration)
	{
		isInvincible = true;
		if (currentShield != null)
		{
			Object.Destroy(currentShield);
		}
		currentShield = Object.Instantiate(shieldPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		currentShield.transform.parent = base.transform;
		currentShield.transform.localPosition = Vector3.zero;
		currentShield.transform.localEulerAngles = Vector3.zero;
		currentShield.transform.localScale = new Vector3(1f, 1f, 1f);
		yield return new WaitForSeconds(duration);
		OnDisableShield();
	}

	public void OnReset()
	{
		StopCoroutine("shieldCountdown");
		CurrentHP = startHealth;
		isInvincible = false;
		DamageShieldValue = 0f;
		_damageMultipliers.Clear();
		if (currentShield != null)
		{
			Object.Destroy(currentShield);
		}
		SendMessageUpwards("OnHealth", CurrentHP / startHealth, SendMessageOptions.DontRequireReceiver);
	}

	public override void OnTakeDamage(float dmg, int shooterID, bool isExplosion, bool isMelee, bool isHeadshot, bool sendNotification, bool endOfGameOverride = false, float radiationDmg = 0f, string customDeathSfx = "")
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		dmg = ApplyMultipliers(dmg);
		radiationDmg = ApplyMultipliers(radiationDmg);
		if (DamageShieldValue > 0f && dmg > 0f && !isHeadshot)
		{
			if (!(DamageShieldValue - dmg < 0f))
			{
				DamageShieldValue -= dmg;
				return;
			}
			dmg -= DamageShieldValue;
			DamageShieldValue = 0f;
		}
		base.OnTakeDamage(dmg, shooterID, isExplosion, isMelee, isHeadshot, sendNotification, endOfGameOverride, radiationDmg, string.Empty);
		if (CurrentHP <= 0f || ((isInvincible || isGrabbingBomb) && !endOfGameOverride))
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
			if (!isShowingHit && base.gameObject.activeInHierarchy && CurrentHP > 0f)
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
		_radiationDmg += radiationDmg;
		if (!playerController.isRemote)
		{
			playerController.SetRadiationDisplay(_radiationDmg / startHealth);
			if (_radiationDmg > 0f)
			{
				StopCoroutine("DelayedHealRadiationDamageOverTime");
				StartCoroutine("DelayedHealRadiationDamageOverTime");
			}
			if (_radiationDmg > startHealth)
			{
				_radiationDmg = 0f;
				playerCharacterManager2.PlayerController.OnLocalRadiationExplode(startHealth, shooterID);
				GameManager.Instance.playerStats[shooterID].addDamageDealt(base.OwnerID, startHealth);
			}
		}
		if (sendNotification && netSyncReporter != null)
		{
			if (playerCharacterManager != null)
			{
				GameManager.Instance.playerStats[shooterID].addDamageDealt(base.OwnerID, dmg);
			}
			StopCoroutine("shieldCountdown");
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = base.OwnerID;
			hashtable[(byte)1] = dmg;
			hashtable[(byte)2] = shooterID;
			netSyncReporter.SetAction(35, hashtable);
		}
		if (CurrentHP <= 0f)
		{
			isInvincible = true;
			bool isSuicide = false;
			if (shooterID == -1 || shooterID == base.OwnerID)
			{
				isSuicide = true;
				shooterID = base.OwnerID;
			}
			HUD.Instance.PlayerController.OnDeath(shooterID, isExplosion, isMelee, isHeadshot, customDeathSfx);
			ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
			hashtable2[(byte)0] = shooterID;
			hashtable2[(byte)1] = isExplosion;
			hashtable2[(byte)2] = isMelee;
			hashtable2[(byte)3] = isHeadshot;
			hashtable2[(byte)4] = customDeathSfx;
			EventTracker.TrackEvent(GameplayEventsHelper.PlayerDied(isSuicide, GameManager.Instance.Players(shooterID)));
			netSyncReporter.SetAction(7, hashtable2);
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

	private float ApplyMultipliers(float damage)
	{
		foreach (float damageMultiplier in _damageMultipliers)
		{
			float num = damageMultiplier;
			damage *= num;
		}
		return damage;
	}
}
