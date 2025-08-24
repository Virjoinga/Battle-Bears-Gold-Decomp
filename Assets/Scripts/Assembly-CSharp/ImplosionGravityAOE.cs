using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ImplosionGravityAOE : ConfigurableNetworkObject
{
	[SerializeField]
	private bool _ignoreOwner;

	[SerializeField]
	private bool _ignoreTeam;

	[SerializeField]
	private LayerMask _mask;

	private float _duration = 3f;

	private float _gravity = 900f;

	private float _timer;

	private SphereCollider _collider;

	private Transform _transform;

	private void Awake()
	{
		_transform = base.transform;
	}

	protected override void Start()
	{
		base.Start();
		_collider = GetComponent<SphereCollider>();
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("duration", ref _duration, equipmentNames);
			itemByName.UpdateProperty("gravity", ref _gravity, equipmentNames);
		}
	}

	private void FixedUpdate()
	{
		if (_timer < _duration)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, _collider.radius, _mask);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				PlayerController component = collider.GetComponent<PlayerController>();
				if (component != null && component.Motor != null)
				{
					Vector3 normalized = (base.transform.position - component.transform.position).normalized;
					Vector3 velocity = normalized * _gravity;
					component.Motor.SetVelocity(velocity);
				}
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		_timer += Time.fixedDeltaTime;
	}
}
