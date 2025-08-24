using System.Collections;
using UnityEngine;

public class SoFly : MeleeAttack
{
	public LayerMask layersToCheck;

	public float flySpeed = 200f;

	public float maxSpeedIncrease = 100f;

	public float flyDuration = 2f;

	private Transform bodyRotator;

	public GameObject attackObject;

	public GameObject flyEffectPrefab;

	private GameObject flyEffect;

	public Vector3 effectOffset;

	private CharacterController character;

	private bool isFlying;

	private Transform rootTransform;

	private CharacterMotor motor;

	private float oldMaxAirAcceleration;

	public override void ConfigureWeapon(Item item)
	{
		item.UpdateProperty("flyingSpeed", ref flySpeed, base.EquipmentNames);
		item.UpdateProperty("maxSpeedIncrease", ref maxSpeedIncrease, base.EquipmentNames);
		item.UpdateProperty("flyingDuration", ref flyDuration, base.EquipmentNames);
		base.ConfigureWeapon(item);
	}

	protected override void Start()
	{
		base.Start();
		if (base.playerController != null)
		{
			bodyRotator = base.playerController.bodyRotator;
		}
		character = myTransform.root.GetComponent(typeof(CharacterController)) as CharacterController;
		OnFindAimer();
		rootTransform = myTransform.root;
		motor = rootTransform.GetComponent(typeof(CharacterMotor)) as CharacterMotor;
		base.enabled = false;
	}

	private void Update()
	{
		if (bodyRotator != null && character != null)
		{
			character.Move(bodyRotator.forward * flySpeed * Time.deltaTime);
		}
	}

	public void OnBombDeactivate()
	{
		if (isFlying)
		{
			StopAllCoroutines();
			StartCoroutine(stopFlying(0f));
		}
	}

	private IEnumerator stopFlying(float stopTime)
	{
		yield return new WaitForSeconds(stopTime);
		base.enabled = false;
		if (motor != null && base.playerController != null)
		{
			motor.movement.gravity = motor.movement.originalGravity;
			base.playerController.canControl = true;
			motor.SetVelocity(Vector3.zero);
			isFlying = false;
		}
		if (flyEffect != null)
		{
			ParticleEmitter p = flyEffect.GetComponentInChildren(typeof(ParticleEmitter)) as ParticleEmitter;
			if (p != null)
			{
				p.emit = false;
			}
		}
	}

	public void OnDestroy()
	{
		if (motor != null && base.playerController != null && isFlying)
		{
			motor.movement.gravity = motor.movement.originalGravity;
			base.playerController.canControl = true;
			motor.SetVelocity(Vector3.zero);
			isFlying = false;
		}
		if (flyEffect != null)
		{
			ParticleEmitter particleEmitter = flyEffect.GetComponentInChildren(typeof(ParticleEmitter)) as ParticleEmitter;
			if (particleEmitter != null)
			{
				particleEmitter.emit = false;
			}
		}
	}

	public override bool OnAttack()
	{
		if (base.playerController == null || bodyRotator == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
			bodyRotator = base.playerController.bodyRotator;
			character = GetComponent<CharacterController>();
			rootTransform = myTransform.root;
			motor = rootTransform.GetComponent<CharacterMotor>();
		}
		if (base.playerController.slowCount > 0)
		{
			return false;
		}
		if (attackObject != null)
		{
			GameObject gameObject = Object.Instantiate(attackObject, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.transform.parent = myTransform;
			gameObject.transform.localPosition = localOffset;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
			component.SetItemOverride(base.name);
			component.SetEquipmentNames(base.EquipmentNames);
			component.OwnerID = ownerID;
			component.DamageMultiplier = base.playerController.DamageMultiplier;
			component.MeleeMultiplier = base.playerController.MeleeMultiplier;
			DelayedDestroy delayedDestroy = gameObject.AddComponent(typeof(DelayedDestroy)) as DelayedDestroy;
			delayedDestroy.delay = flyDuration;
			StartCoroutine(stopFlying(flyDuration));
			Physics.IgnoreCollision(gameObject.collider, rootTransform.collider);
		}
		if (flyEffectPrefab != null)
		{
			flyEffect = Object.Instantiate(flyEffectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			flyEffect.transform.parent = bodyRotator;
			flyEffect.transform.localPosition = effectOffset;
			flyEffect.transform.localScale = new Vector3(1f, 1f, 1f);
			StartCoroutine(delayedEffectDisable(flyDuration));
		}
		if (!isFlying)
		{
			motor.movement.gravity = 0f;
			base.playerController.StatManager.AddStatMod(new StatisticMod(Statistic.MaxForwardMovementSpeed, flyDuration, maxSpeedIncrease));
			motor.SetVelocity(Vector3.zero);
			motor.movement.frameVelocity = Vector3.zero;
			motor.inputMoveDirection = Vector3.zero;
			base.playerController.canControl = false;
			base.enabled = true;
			isFlying = true;
		}
		base.NetSyncReporter.SpawnProjectile(Vector3.zero, Vector3.zero);
		return true;
	}

	public override void OnRemoteAttack(Vector3 pos, Vector3 vel, int delay)
	{
		if (base.playerController == null || bodyRotator == null)
		{
			base.playerController = myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
			bodyRotator = base.playerController.bodyRotator;
			character = GetComponent(typeof(CharacterController)) as CharacterController;
			rootTransform = myTransform.root;
			motor = rootTransform.GetComponent(typeof(CharacterMotor)) as CharacterMotor;
		}
		if (attackObject != null)
		{
			GameObject gameObject = Object.Instantiate(attackObject, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.transform.parent = myTransform;
			gameObject.transform.localPosition = localOffset;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			ConfigurableNetworkObject component = gameObject.GetComponent<ConfigurableNetworkObject>();
			component.SetItemOverride(base.name);
			component.SetEquipmentNames(base.EquipmentNames);
			component.OwnerID = ownerID;
			component.DamageMultiplier = base.playerController.DamageMultiplier;
			component.MeleeMultiplier = base.playerController.MeleeMultiplier;
			DelayedDestroy delayedDestroy = gameObject.AddComponent(typeof(DelayedDestroy)) as DelayedDestroy;
			delayedDestroy.delay = flyDuration - (float)delay / 1000f;
			Physics.IgnoreCollision(gameObject.collider, rootTransform.collider);
		}
		if (flyEffectPrefab != null)
		{
			flyEffect = Object.Instantiate(flyEffectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			flyEffect.transform.parent = bodyRotator;
			flyEffect.transform.localPosition = effectOffset;
			flyEffect.transform.localScale = new Vector3(1f, 1f, 1f);
			StartCoroutine(delayedEffectDisable(flyDuration - (float)delay / 1000f));
		}
	}

	private IEnumerator delayedEffectDisable(float time)
	{
		yield return new WaitForSeconds(time);
		if (flyEffect != null)
		{
			ParticleEmitter p = flyEffect.GetComponentInChildren(typeof(ParticleEmitter)) as ParticleEmitter;
			if (p != null)
			{
				p.emit = false;
			}
		}
	}
}
