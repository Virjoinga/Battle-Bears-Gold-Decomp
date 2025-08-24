using UnityEngine;

public class PrefabSpawnerWeapon : WeaponBase
{
	private PrefabSpawner _spawner;

	private float _lastStartTime;

	private float _prefabDamage = 25f;

	protected override void Awake()
	{
		base.Awake();
		_spawner = GetComponent<PrefabSpawner>();
		_spawner.Weapon = this;
	}

	protected override void Start()
	{
		base.Start();
		if (base.playerController.CharacterManager != null)
		{
			if (base.playerController.CharacterManager.playerLoadout.primary.name.Equals(base.name))
			{
				_prefabDamage = (float)base.playerController.CharacterManager.playerLoadout.primary.properties["damage"];
			}
			else if (base.playerController.CharacterManager.playerLoadout.secondary.name.Equals(base.name))
			{
				_prefabDamage = (float)base.playerController.CharacterManager.playerLoadout.secondary.properties["damage"];
			}
		}
		_spawner.PrefabDamage = _prefabDamage;
		SetAnimationLayers();
	}

	public override bool OnAttack()
	{
		EnableSpawner();
		base.playerController.canSwitchWeapons = false;
		return base.OnAttack();
	}

	public void TellPrefabSpawned(Vector3 position, Vector3 velocity)
	{
		if (base.NetSyncReporter != null && !dontSendNetworkMessages)
		{
			base.NetSyncReporter.SpawnProjectile(position, velocity);
		}
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		base.playerController.canSwitchWeapons = false;
		if (_spawner != null)
		{
			_spawner.SpawnPrefab(pos, vel, delay, true);
		}
		if (Time.fixedTime - _lastStartTime > firingTime)
		{
			base.OnRemoteAttack(pos, vel, delay);
			_lastStartTime = Time.fixedTime;
		}
	}

	public override void PlayReloadAnimation()
	{
		if (base.playerController != null && !base.playerController.IsDead)
		{
			CancelInvoke("DisableSpawner");
			DisableSpawner();
			base.PlayReloadAnimation();
		}
	}

	public override void WeaponDeath()
	{
		base.WeaponDeath();
		DisableSpawner();
	}

	private void SetAnimationLayers()
	{
		if (base.playerController.BodyAnimator.Animator[base.name + "_fireIn"] != null)
		{
			base.playerController.BodyAnimator.Animator[base.name + "_fireIn"].layer = 0;
		}
		if (base.playerController.BodyAnimator.Animator[base.name + "_fireLoop"] != null)
		{
			base.playerController.BodyAnimator.Animator[base.name + "_fireLoop"].layer = 0;
		}
		if (base.playerController.BodyAnimator.Animator[base.name + "_fireOut"] != null)
		{
			base.playerController.BodyAnimator.Animator[base.name + "_fireOut"].layer = 0;
		}
	}

	private void EnableSpawner()
	{
		CancelInvoke("DisableSpawner");
		_spawner.EnableSpawning = true;
		Invoke("DisableSpawner", firingTime);
	}

	private void DisableSpawner()
	{
		if (_spawner != null)
		{
			_spawner.EnableSpawning = false;
		}
		if (base.playerController != null)
		{
			base.playerController.canSwitchWeapons = true;
		}
	}
}
