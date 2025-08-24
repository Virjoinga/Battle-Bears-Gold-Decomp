using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
	[SerializeField]
	private string _prefabResourceDirectory;

	[SerializeField]
	private string[] _prefabNames = new string[4];

	[SerializeField]
	private float _spawnRate = 0.5f;

	[SerializeField]
	private Transform _spawnPosition;

	[SerializeField]
	private Vector3 _initialVelocity;

	[SerializeField]
	private Vector3 _angularVelocity;

	[SerializeField]
	private float _lifetime = 3f;

	private float _lastSpawnTime;

	private int _currentPrefabIndex;

	private PlayerController _playerController;

	private PrefabSpawnerWeapon _weapon;

	private float _prefabDamage;

	private bool _enableSpawning;

	public PrefabSpawnerWeapon Weapon
	{
		get
		{
			return _weapon;
		}
		set
		{
			_weapon = value;
		}
	}

	public float PrefabDamage
	{
		get
		{
			return _prefabDamage;
		}
		set
		{
			_prefabDamage = value;
		}
	}

	public bool EnableSpawning
	{
		get
		{
			return _enableSpawning;
		}
		set
		{
			_enableSpawning = value;
		}
	}

	private void Awake()
	{
		_playerController = base.gameObject.transform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
	}

	private void Update()
	{
		if (_enableSpawning && Time.fixedTime - _lastSpawnTime > _spawnRate)
		{
			SpawnPrefab(_spawnPosition.transform.position, _initialVelocity);
		}
	}

	public void SpawnPrefab(Vector3 position, Vector3 velocity, long delay = 0, bool isRemote = false)
	{
		if (_playerController == null)
		{
			_playerController = base.transform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
		}
		if (!_playerController.IsDead)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load(_prefabResourceDirectory + _prefabNames[_currentPrefabIndex])) as GameObject;
			Transform transform = gameObject.transform.Find("DamageCollider");
			if (transform != null && _playerController != null)
			{
				Physics.IgnoreCollision(transform.gameObject.GetComponent<Collider>(), _playerController.GetComponent<Collider>());
			}
			gameObject.BroadcastMessage("SetEquipmentNames", _weapon.EquipmentNames, SendMessageOptions.DontRequireReceiver);
			gameObject.BroadcastMessage("SetItemOverride", _weapon.name, SendMessageOptions.DontRequireReceiver);
			PrefabProjectile componentInChildren = gameObject.GetComponentInChildren<PrefabProjectile>();
			componentInChildren.OwnerID = _weapon.OwnerID;
			componentInChildren.DamageMultiplier = _playerController.DamageMultiplier;
			componentInChildren.damage = _prefabDamage;
			gameObject.transform.position = position + velocity * delay / 1000f;
			if (!isRemote)
			{
				gameObject.transform.rotation = _spawnPosition.rotation;
			}
			gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.rotation * velocity;
			gameObject.GetComponent<Rigidbody>().angularVelocity = _angularVelocity;
			if (!isRemote)
			{
				_weapon.TellPrefabSpawned(gameObject.transform.position, gameObject.GetComponent<Rigidbody>().velocity);
			}
			Object.Destroy(gameObject, _lifetime);
			_currentPrefabIndex = ((_currentPrefabIndex + 1 < _prefabNames.Length) ? (_currentPrefabIndex + 1) : 0);
			_lastSpawnTime = Time.fixedTime;
		}
	}
}
