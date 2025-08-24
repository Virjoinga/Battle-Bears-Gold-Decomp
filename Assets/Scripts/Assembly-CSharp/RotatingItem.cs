using UnityEngine;

public class RotatingItem : MonoBehaviour
{
	[SerializeField]
	private float _rotationsPerSecond = 2f;

	[SerializeField]
	protected string _itemOverride;

	[SerializeField]
	private bool _getDurationFromServer;

	[SerializeField]
	private bool _destroyOnExpire;

	[SerializeField]
	private bool _rotateAroundXAxis;

	[SerializeField]
	private bool _rotateAroundYAxis;

	[SerializeField]
	private bool _rotateAroundZAxis;

	private static readonly string _expiredMethodName = "Expired";

	private static readonly string _durationStr = "duration";

	private float _rotSpeed;

	private float _persistenceLength = 7f;

	public virtual void Start()
	{
		_rotSpeed = _rotationsPerSecond * 360f;
		if (ServiceManager.Instance != null && _getDurationFromServer)
		{
			Item itemByName = ServiceManager.Instance.GetItemByName(_itemOverride);
			if (itemByName != null && itemByName.properties.ContainsKey(_durationStr))
			{
				_persistenceLength = (float)itemByName.properties[_durationStr];
			}
		}
		Invoke(_expiredMethodName, _persistenceLength);
	}

	public virtual void Update()
	{
		RotateForFrame();
	}

	private void RotateForFrame()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (_rotateAroundXAxis)
		{
			localEulerAngles.x += Time.deltaTime * _rotSpeed;
		}
		if (_rotateAroundYAxis)
		{
			localEulerAngles.y += Time.deltaTime * _rotSpeed;
		}
		if (_rotateAroundZAxis)
		{
			localEulerAngles.z += Time.deltaTime * _rotSpeed;
		}
		base.transform.localEulerAngles = localEulerAngles;
	}

	protected virtual void Expired()
	{
		if (_destroyOnExpire)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
