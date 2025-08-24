using System.Collections;
using UnityEngine;

public class HomingMissile : Projectile
{
	[SerializeField]
	private float descendSpeed = 1000f;

	[SerializeField]
	private float ascendSpeed = 1000f;

	[SerializeField]
	private float ascendTime = 3f;

	[SerializeField]
	private float homingRating = 7f;

	private Transform _transform;

	private Rigidbody _rigidbody;

	private Vector3 _target;

	private Transform _lockedTarget;

	private bool hasValuesSet;

	private void Awake()
	{
		_transform = base.transform;
		_rigidbody = base.rigidbody;
		base.collider.enabled = false;
		StartCoroutine(DelayedEnableCollider(0.2f));
		_transform.forward = Vector3.up;
	}

	private new void Start()
	{
		if (ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("descendSpeed", ref descendSpeed, base.EquipmentNames);
			itemByName.UpdateProperty("ascendSpeed", ref ascendSpeed, base.EquipmentNames);
			itemByName.UpdateProperty("ascendTime", ref ascendTime, base.EquipmentNames);
			itemByName.UpdateProperty("homingRating", ref homingRating, base.EquipmentNames);
			hasValuesSet = true;
		}
	}

	public IEnumerator DelayedEnableCollider(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		base.collider.enabled = true;
	}

	public void StartHomingCountDown(Vector3 t, float delay, Transform lockedTarget = null)
	{
		_target = t;
		_lockedTarget = lockedTarget;
		StartCoroutine(delayedHoming(delay));
	}

	private IEnumerator delayedHoming(float delay)
	{
		while (!hasValuesSet)
		{
			yield return new WaitForSeconds(0.05f);
		}
		_rigidbody.velocity = Vector3.up * ascendSpeed;
		_transform.position += _rigidbody.velocity * (delay / 1000f);
		float ascendDelay = ascendTime - delay / 1000f;
		if (ascendDelay > 0f)
		{
			yield return new WaitForSeconds(ascendDelay);
		}
		while (_transform != null)
		{
			if (_lockedTarget != null)
			{
				_target = _lockedTarget.transform.position;
			}
			Vector3 targetDirection = _target - _transform.position;
			_transform.forward = Vector3.RotateTowards(_transform.forward, targetDirection, Time.deltaTime * homingRating, 0f);
			_rigidbody.velocity = _transform.forward * descendSpeed;
			yield return null;
		}
	}

	private void LateUpdate()
	{
		_transform.LookAt(_transform.position + _rigidbody.velocity);
	}
}
