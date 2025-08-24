using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectField : ConfigurableNetworkObject
{
	private static readonly string _expiredMethodName = "Expired";

	[SerializeField]
	private float _effectRadius;

	[SerializeField]
	private bool _scaleField;

	[SerializeField]
	private float _scaleSpeedPerSecond = 0.25f;

	private List<PlayerController> _affectedPlayers = new List<PlayerController>();

	private Vector3 _targetScale;

	private float _adjustedScaleSpeedPerSecond;

	protected float _duration = 7f;

	protected override void Start()
	{
		base.Start();
		_targetScale = new Vector3(_effectRadius, _effectRadius, _effectRadius);
		_adjustedScaleSpeedPerSecond = _effectRadius * _scaleSpeedPerSecond;
		if (_scaleField)
		{
			base.transform.localScale = Vector3.zero;
		}
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("duration", ref _duration, equipmentNames);
		}
		Invoke(_expiredMethodName, _duration);
	}

	private void AddPlayersInCollision()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, _effectRadius);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			PlayerController component = collider.gameObject.GetComponent<PlayerController>();
			if (component != null)
			{
				ApplyEffect(component);
			}
		}
	}

	protected virtual void Update()
	{
		ScaleUpForFrame();
	}

	protected virtual void ApplyEffect(PlayerController pc)
	{
		_affectedPlayers.Add(pc);
	}

	protected virtual void RemoveEffect(PlayerController pc)
	{
		_affectedPlayers.Remove(pc);
	}

	private void OnTriggerEnter(Collider other)
	{
		PlayerController component = other.gameObject.GetComponent<PlayerController>();
		if (component != null)
		{
			ApplyEffect(component);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		PlayerController component = other.gameObject.GetComponent<PlayerController>();
		if (component != null)
		{
			RemoveEffect(component);
		}
	}

	private void OnDestroy()
	{
		base.transform.localScale = Vector3.zero;
	}

	private void ScaleUpForFrame()
	{
		if (!_scaleField)
		{
			return;
		}
		float num = Time.deltaTime * _adjustedScaleSpeedPerSecond;
		Vector3 localScale = base.transform.localScale;
		localScale.x += num;
		localScale.y += num;
		localScale.z += num;
		if (num > 0f)
		{
			if (localScale.x >= _targetScale.x)
			{
				base.transform.localScale = _targetScale;
				_scaleField = false;
			}
			else
			{
				base.transform.localScale = localScale;
			}
		}
		else if (localScale.x <= 0f)
		{
			base.transform.localScale = Vector3.zero;
			_scaleField = false;
		}
		else
		{
			base.transform.localScale = localScale;
		}
	}

	protected virtual void Expired()
	{
		StartCoroutine(ShrinkAndDestroyRoutine());
	}

	private IEnumerator ShrinkAndDestroyRoutine()
	{
		_scaleField = true;
		_adjustedScaleSpeedPerSecond = 0f - _adjustedScaleSpeedPerSecond;
		while (base.transform.localScale != Vector3.zero)
		{
			yield return null;
		}
		RemoveAllAffectedPlayers();
		Object.Destroy(base.gameObject);
	}

	private void RemoveAllAffectedPlayers()
	{
		PlayerController[] array = _affectedPlayers.ToArray();
		foreach (PlayerController playerController in array)
		{
			if (playerController != null)
			{
				RemoveEffect(playerController);
			}
		}
		_affectedPlayers.Clear();
	}
}
