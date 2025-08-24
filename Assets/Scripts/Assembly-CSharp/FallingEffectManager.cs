using UnityEngine;

public class FallingEffectManager : MonoBehaviour
{
	[SerializeField]
	private CharacterMotor _motor;

	[SerializeField]
	private ParticleSystem _particleSystemToSpawn;

	[SerializeField]
	private Transform _spawnPoint;

	private ParticleSystem _currentEffect;

	private void Start()
	{
		_currentEffect = Object.Instantiate(_particleSystemToSpawn) as ParticleSystem;
		_currentEffect.transform.parent = _spawnPoint;
		_currentEffect.transform.localPosition = Vector3.zero;
		_currentEffect.transform.localRotation = Quaternion.identity;
		_currentEffect.enableEmission = false;
	}

	private void Update()
	{
		if (!_currentEffect.enableEmission && !_motor.grounded && _motor.movement.velocity.y < 0f)
		{
			_currentEffect.enableEmission = true;
		}
		else if (_motor.grounded || _motor.movement.velocity.y > 0f)
		{
			_currentEffect.enableEmission = false;
		}
	}
}
