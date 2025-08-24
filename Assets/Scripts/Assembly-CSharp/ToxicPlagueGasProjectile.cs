using UnityEngine;

public class ToxicPlagueGasProjectile : ConfigurableNetworkObject
{
	[SerializeField]
	private GameObject _gas;

	[SerializeField]
	private float _timeToWaitBeforeFirstCloud;

	private Transform _transform;

	private int _numToxicTrailsColliding;

	private float _gasColliderRadius;

	private Vector3 _positionWhenLeftLastGasCloud;

	private void Awake()
	{
		_transform = base.transform;
		_gasColliderRadius = _gas.GetComponent<SphereCollider>().radius;
		_positionWhenLeftLastGasCloud = _transform.position + new Vector3(_gasColliderRadius, _gasColliderRadius, _gasColliderRadius);
	}

	private void Update()
	{
		TrySpawnNewTrail();
	}

	private void TrySpawnNewTrail()
	{
		if (ShouldSpawnNewTrail())
		{
			SetupNewTrail();
		}
	}

	private bool ShouldSpawnNewTrail()
	{
		return !InsideToxicTrail() && IsRadiusDistanceAwayFromLastGasCloud();
	}

	private bool InsideToxicTrail()
	{
		return _numToxicTrailsColliding > 0;
	}

	private bool IsRadiusDistanceAwayFromLastGasCloud()
	{
		float num = Vector3.Distance(_transform.position, _positionWhenLeftLastGasCloud);
		return num > _gasColliderRadius;
	}

	private void SetupNewTrail()
	{
		GameObject gasClone = Object.Instantiate(_gas, _transform.position, Quaternion.identity) as GameObject;
		SetupNetworkVariables(gasClone);
	}

	private void SetupNetworkVariables(GameObject gasClone)
	{
		ConfigurableNetworkObject component = gasClone.GetComponent<ConfigurableNetworkObject>();
		ForwardSettings(component);
		component.OwnerID = base.OwnerID;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.name == _gas.name + "(Clone)")
		{
			_numToxicTrailsColliding++;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.name == _gas.name + "(Clone)")
		{
			_numToxicTrailsColliding--;
			if (_numToxicTrailsColliding <= 0)
			{
				_positionWhenLeftLastGasCloud = _transform.position;
			}
		}
	}
}
