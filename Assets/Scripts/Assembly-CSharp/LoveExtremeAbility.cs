using UnityEngine;

public class LoveExtremeAbility : SpecialAbility
{
	public float damageMultiplier = 1.5f;

	private Transform myTransform;

	private GameObject loveEffect;

	protected override void Awake()
	{
		base.Awake();
		myTransform = base.transform;
	}

	protected override void Start()
	{
		base.Start();
		if (effectPrefab != null)
		{
			loveEffect = Object.Instantiate(effectPrefab) as GameObject;
			loveEffect.transform.parent = myTransform;
			loveEffect.transform.localPosition = Vector3.zero;
			loveEffect.transform.localEulerAngles = Vector3.zero;
			loveEffect.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			loveEffect = null;
		}
		playerController.MeleeMultiplier = damageMultiplier;
	}

	private void OnDisable()
	{
		OnEndEffect();
	}

	private void OnEndEffect()
	{
		if (loveEffect != null)
		{
			playerController.MeleeMultiplier = 1f;
			ParticleEmitter particleEmitter = loveEffect.GetComponentInChildren(typeof(ParticleEmitter)) as ParticleEmitter;
			if (particleEmitter != null)
			{
				particleEmitter.emit = false;
			}
			DelayedDestroy delayedDestroy = loveEffect.AddComponent(typeof(DelayedDestroy)) as DelayedDestroy;
			if (delayedDestroy != null)
			{
				delayedDestroy.delay = 1.25f;
			}
			loveEffect = null;
		}
		Object.Destroy(this);
	}
}
