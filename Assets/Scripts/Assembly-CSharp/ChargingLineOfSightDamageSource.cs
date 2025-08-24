using UnityEngine;

public class ChargingLineOfSightDamageSource : LineOfSightDamageSource
{
	public ParticleSystem[] particles;

	private float _minDamage;

	private float _maxDamage;

	private float _minDamageRange;

	private float _maxDamageRange;

	private float _charge;

	private bool _startHasRun;

	private BoxCollider _myCollider;

	public float ChargePercent
	{
		get
		{
			return _charge;
		}
		set
		{
			_charge = value;
			if (_startHasRun)
			{
				SetDamageAndScaleFromCharge(value);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_myCollider = GetComponent<BoxCollider>();
	}

	protected override void Start()
	{
		base.Start();
		if (configureItemName != string.Empty && ServiceManager.Instance != null)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(configureItemName);
			itemByName.UpdateProperty("minDamage", ref _minDamage, equipmentNames);
			itemByName.UpdateProperty("maxDamage", ref _maxDamage, equipmentNames);
			itemByName.UpdateProperty("minDamageRange", ref _minDamageRange, equipmentNames);
			itemByName.UpdateProperty("maxDamageRange", ref _maxDamageRange, equipmentNames);
		}
		_startHasRun = true;
		SetDamageAndScaleFromCharge(ChargePercent);
	}

	private void SetDamageAndScaleFromCharge(float charge)
	{
		damage = Mathf.Lerp(_minDamage, _maxDamage, _charge);
		float num = Mathf.Lerp(_minDamageRange, _maxDamageRange, _charge);
		Vector3 size = new Vector3(num * 0.6f, num * 0.3f, num);
		Vector3 center = new Vector3(0f, 0f, num * 0.5f);
		_myCollider.size = size;
		_myCollider.center = center;
		float to = _maxDamageRange / _minDamageRange;
		float num2 = Mathf.Lerp(1f, to, _charge);
		foreach (Transform item in base.transform)
		{
			item.localScale = new Vector3(item.localScale.x * num2, item.localScale.y * num2, item.localScale.z * num2);
		}
		if (particles != null)
		{
			ParticleSystem[] array = particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].startSize *= num2;
			}
		}
	}
}
