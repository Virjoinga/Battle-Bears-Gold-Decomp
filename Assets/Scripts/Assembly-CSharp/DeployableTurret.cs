using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class DeployableTurret : DeployableObject
{
	public AudioClip[] fireSounds;

	public AudioClip[] deathSounds;

	public AudioClip assembleSound;

	public GameObject deathEffect;

	public bool matchesOwnerTeamColor;

	public Vector2 uvOffsetForBlue;

	private PlayerController _owningPlayer;

	private TurretTargettingSystem _targetingSystem;

	private bool _turretIsReady;

	private float cooldown = 0.2f;

	private bool _turretIsDead;

	public Transform spawnPoint;

	public float raycastOffset = 30f;

	public LayerMask layersToHit;

	public float damage = 1f;

	public float dispersionAngleAmount;

	public float tracerToggleTime = 0.1f;

	public GameObject tracerPrefab;

	private GameObject tracer;

	private Transform tracerStart;

	private Transform tracerEnd;

	private Renderer tracerMesh;

	private Component[] tracerHeadMeshes;

	private Transform tracerHead;

	private bool showingTracer;

	private Transform bodyRotator;

	private Transform tracerTransform;

	private AudioSource myAudio;

	public override PlayerController OwningPlayer
	{
		get
		{
			return _owningPlayer;
		}
		set
		{
			_owningPlayer = value;
			TargetableObject componentInChildren = GetComponentInChildren<TargetableObject>();
			if (componentInChildren != null)
			{
				componentInChildren.Team = _owningPlayer.Team;
			}
			TurretDamageReceiver componentInChildren2 = GetComponentInChildren<TurretDamageReceiver>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.OwnerID = _owningPlayer.OwnerID;
				componentInChildren2.OwningPlayer = _owningPlayer;
				componentInChildren2.turretIndex = deployableIndex;
				componentInChildren2.weaponIndex = weaponIndex;
				componentInChildren2.isRemote = _owningPlayer.isRemote;
			}
			if (!matchesOwnerTeamColor || value.Team != Team.BLUE)
			{
				return;
			}
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] array = componentsInChildren;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				if (!(tracerMesh != null) || !(tracerMesh == skinnedMeshRenderer))
				{
					Mesh mesh = Object.Instantiate(skinnedMeshRenderer.sharedMesh) as Mesh;
					Vector2[] uv = mesh.uv;
					for (int j = 0; j < uv.Length; j++)
					{
						uv[j].x += uvOffsetForBlue.x;
						uv[j].y += uvOffsetForBlue.y;
					}
					mesh.uv = uv;
					skinnedMeshRenderer.sharedMesh = mesh;
				}
			}
		}
	}

	public bool IsDead
	{
		get
		{
			return _turretIsDead;
		}
	}

	protected void Awake()
	{
		if (tracerPrefab != null)
		{
			if (spawnPoint == null)
			{
				spawnPoint = base.transform.Find("turretAimer");
			}
			tracer = Object.Instantiate(tracerPrefab) as GameObject;
			tracerStart = tracer.transform.Find("start");
			tracerEnd = tracer.transform.Find("end");
			tracerMesh = tracer.transform.Find("mesh").renderer;
			tracerMesh.enabled = false;
			tracerTransform = tracer.transform;
			tracerTransform.parent = spawnPoint;
			tracerTransform.localPosition = Vector3.zero;
			tracerTransform.localScale = new Vector3(1f, 1f, 1f);
			tracerHead = tracerEnd.Find("Tracer_Head");
			tracerHeadMeshes = tracerHead.GetComponentsInChildren(typeof(Renderer));
			Component[] array = tracerHeadMeshes;
			foreach (Component component in array)
			{
				(component as Renderer).enabled = false;
			}
		}
		if (deathEffect != null)
		{
			deathEffect.SetActive(false);
		}
		myAudio = base.audio;
		myAudio.PlayOneShot(assembleSound);
	}

	public override void ConfigureObject()
	{
		base.ConfigureObject();
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("damage", ref damage, base.EquipmentNames);
			itemByName.UpdateProperty("cooldown", ref cooldown, base.EquipmentNames);
			itemByName.UpdateProperty("dispersionAngle", ref dispersionAngleAmount, base.EquipmentNames);
		}
	}

	public void OnDestroy()
	{
		if (tracer != null)
		{
			Object.Destroy(tracer.gameObject);
		}
	}

	public bool OnAttack()
	{
		bool hasHitSomething = false;
		if (bodyRotator == null)
		{
			bodyRotator = _targetingSystem.aimer;
		}
		bodyRotator.LookAt(_targetingSystem.lockedTarget.position);
		Vector3 vector = bodyRotator.TransformDirection(new Vector3((1f - 2f * Random.value) * dispersionAngleAmount, (1f - 2f * Random.value) * dispersionAngleAmount, 1f));
		RaycastHit hitInfo;
		if (Physics.Raycast(bodyRotator.position, vector, out hitInfo, 4000f, layersToHit))
		{
			if (tracerEnd != null)
			{
				tracerEnd.position = hitInfo.point;
				hasHitSomething = true;
			}
			Transform transform = hitInfo.transform;
			DamageReceiver damageReceiver = transform.GetComponent(typeof(DamageReceiver)) as DamageReceiver;
			if (damageReceiver != null && !damageReceiver.isInvincible)
			{
				OnDealDirectDamage(damageReceiver, damage * OwningPlayer.DamageMultiplier);
			}
		}
		else if (tracerEnd != null)
		{
			tracerEnd.position = spawnPoint.position + vector * 4000f;
			hasHitSomething = false;
		}
		if (tracerMesh != null && !showingTracer)
		{
			StartCoroutine(showTracer(tracerEnd.position, hitInfo.normal, hasHitSomething));
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[(byte)0] = OwningPlayer.OwnerID;
		hashtable[(byte)1] = weaponIndex;
		hashtable[(byte)2] = deployableIndex;
		hashtable[(byte)3] = tracerEnd.position.x;
		hashtable[(byte)4] = tracerEnd.position.y;
		hashtable[(byte)5] = tracerEnd.position.z;
		OwningPlayer.NetSync.SetAction(42, hashtable);
		base.animation.Play(base.name + "_fire");
		myAudio.PlayOneShot(fireSounds[Random.Range(0, fireSounds.Length)]);
		return true;
	}

	public void OnRemoteAttack(Vector3 pos)
	{
		StopCoroutine("DelayedStopFiringAnimation");
		base.animation.Play(base.name + "_fire");
		myAudio.PlayOneShot(fireSounds[Random.Range(0, fireSounds.Length)]);
		StartCoroutine("DelayedStopFiringAnimation");
		if (tracerMesh != null && !showingTracer)
		{
			spawnPoint.LookAt(pos);
			StartCoroutine(showTracer(pos, Vector3.zero, false));
		}
	}

	protected IEnumerator DelayedStopFiringAnimation()
	{
		yield return new WaitForSeconds(0.6f);
		base.animation.Stop(base.name + "_fire");
	}

	protected void OnDealDirectDamage(DamageReceiver dmgReceiver, float damage)
	{
		if (GameManager.Instance == null)
		{
			dmgReceiver.OnTakeDamage(0f, -1, false, false, false, false, false, 0f, string.Empty);
			return;
		}
		PlayerCharacterManager playerCharacterManager = GameManager.Instance.Players(base.OwnerID);
		PlayerCharacterManager playerCharacterManager2 = GameManager.Instance.Players(dmgReceiver.OwnerID);
		if (OwningPlayer.NetSync != null && (!(GameManager.Instance.friendlyFireRatio < 0.01f) || !(playerCharacterManager != null) || !(playerCharacterManager2 != null) || !(dmgReceiver != null) || dmgReceiver.OwnerID == base.OwnerID || playerCharacterManager.team != playerCharacterManager2.team))
		{
			dmgReceiver.OnTakeDamage(damage, base.OwnerID, false, false, false, false, false, 0f, string.Empty);
			if (playerCharacterManager != null && playerCharacterManager2 != null && dmgReceiver != null && playerCharacterManager.team == playerCharacterManager2.team && dmgReceiver.OwnerID != base.OwnerID)
			{
				damage *= GameManager.Instance.friendlyFireRatio;
			}
			GameManager.Instance.playerStats[base.OwnerID].addDamageDealt(dmgReceiver.OwnerID, damage);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = dmgReceiver.OwnerID;
			hashtable[(byte)1] = damage;
			hashtable[(byte)2] = base.OwnerID;
			if (dmgReceiver is PlayerDamageReceiver)
			{
				OwningPlayer.NetSync.SetAction(29, hashtable);
			}
			else if (dmgReceiver is TurretDamageReceiver)
			{
				hashtable[(byte)3] = ((TurretDamageReceiver)dmgReceiver).turretIndex;
				OwningPlayer.NetSync.SetAction(30, hashtable);
			}
		}
	}

	private IEnumerator showTracer(Vector3 endPos, Vector3 hitNormal, bool hasHitSomething)
	{
		showingTracer = true;
		tracerTransform.localScale = new Vector3(1f, 1f, 1f);
		tracerTransform.parent = tracerTransform.root;
		tracerStart.position = spawnPoint.position;
		tracerEnd.position = endPos;
		tracerStart.LookAt(tracerEnd);
		tracerEnd.eulerAngles = tracerStart.eulerAngles;
		tracerMesh.enabled = true;
		if (hasHitSomething)
		{
			Component[] array = tracerHeadMeshes;
			foreach (Component c2 in array)
			{
				(c2 as Renderer).enabled = true;
			}
			float newScale = Random.Range(0.75f, 1.25f);
			tracerEnd.localScale = new Vector3(newScale, newScale, newScale);
			tracerHead.LookAt(endPos);
			Vector3 rot = tracerHead.eulerAngles;
			rot.z = Random.Range(0, 360);
			tracerHead.eulerAngles = rot;
		}
		yield return new WaitForSeconds(tracerToggleTime);
		tracerMesh.enabled = false;
		tracerTransform.parent = spawnPoint;
		Component[] array2 = tracerHeadMeshes;
		foreach (Component c in array2)
		{
			(c as Renderer).enabled = false;
		}
		yield return new WaitForSeconds(tracerToggleTime);
		showingTracer = false;
	}

	protected new void Start()
	{
		if (OwningPlayer.Team == Team.RED)
		{
			base.transform.Find("playerHighlight").renderer.sharedMaterial = Resources.Load("goggleMaterialRed") as Material;
		}
		else
		{
			base.transform.Find("playerHighlight").renderer.sharedMaterial = Resources.Load("goggleMaterialBlue") as Material;
		}
		base.name = base.name.Replace("(Clone)", string.Empty);
		_targetingSystem = GetComponent<TurretTargettingSystem>();
		AnimationClip clip = base.animation.GetClip(base.name + "_ready");
		StartCoroutine(base.animation.playWithCallbackCoroutine(clip, delegate
		{
			if (!OwningPlayer.isRemote)
			{
				_turretIsReady = true;
				StartCoroutine(AttackingBehavior());
			}
		}));
		ConfigureObject();
	}

	public override void OnDetonateDeployable(PlayerController triggeringPlayer, bool fromExplosion)
	{
		if (!_turretIsDead)
		{
			if (OwningPlayer != null && triggeringPlayer.NetSync != null && weaponIndex != -1)
			{
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[(byte)0] = OwningPlayer.OwnerID;
				hashtable[(byte)1] = weaponIndex;
				hashtable[(byte)2] = deployableIndex;
				triggeringPlayer.NetSync.SetAction(38, hashtable);
			}
			OnDestroyDeployable();
		}
	}

	public override void OnDestroyDeployable()
	{
		if (_turretIsDead)
		{
			return;
		}
		AnimationClip clip = base.animation.GetClip(base.name + "_death");
		_turretIsReady = false;
		_turretIsDead = true;
		Object.Destroy(base.transform.Find("playerHighlight").gameObject);
		StopAllCoroutines();
		myAudio.PlayOneShot(deathSounds[Random.Range(0, fireSounds.Length)]);
		if (deathEffect != null)
		{
			deathEffect.SetActive(true);
		}
		StartCoroutine(base.animation.playWithCallbackCoroutine(clip, delegate
		{
			if (objectToSpawn != null && !hasSpawned)
			{
				GameObject gameObject = Object.Instantiate(objectToSpawn, base.transform.position, Quaternion.identity) as GameObject;
				ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
				if (component != null)
				{
					component.OwnerID = base.OwnerID;
					component.DamageMultiplier = base.DamageMultiplier;
					component.SetItemOverride(spawnItemOverride);
					component.SetEquipmentNames(equipmentNames);
				}
				hasSpawned = true;
			}
			if (OwningPlayer.WeaponManager.CurrentWeaponIndex == weaponIndex)
			{
				int num = 1;
				WeaponBase currentWeapon = OwningPlayer.WeaponManager.CurrentWeapon;
				OwningPlayer.WeaponManager.OnDelayedIncreaseAmmo(currentWeapon.reloadTime * (float)num / (float)currentWeapon.clipSize, num);
			}
			StartCoroutine(DelayedDestroy());
		}));
	}

	private IEnumerator AttackingBehavior()
	{
		while (true)
		{
			if (_targetingSystem.lockedTarget == null)
			{
				base.animation.Stop(base.name + "_fire");
				yield return new WaitForSeconds(cooldown);
			}
			else if (_turretIsReady)
			{
				OnAttack();
				yield return new WaitForSeconds(cooldown);
			}
		}
	}

	private IEnumerator DelayedDestroy()
	{
		yield return new WaitForSeconds(3f);
		if (base.transform.root != null)
		{
			Object.Destroy(base.transform.root.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
