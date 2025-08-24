using System;
using ExitGames.Client.Photon;
using UnityEngine;

public class DiscController : RotatingItem
{
	[SerializeField]
	private GameObject _explosionEffect;

	[SerializeField]
	private Vector3 _floatOffset;

	[SerializeField]
	private float _floatBobHeight;

	[SerializeField]
	private float _maxBobSpeedPerSecond;

	[SerializeField]
	protected Transform[] _spawnPoints;

	private float _bobVelocity;

	private float _bobOffset;

	private float _halfBobHeight;

	private int _spawnIndex;

	public bool IsRemote { get; set; }

	public PlayerController Owner { get; set; }

	public int DiscIndex { get; set; }

	public DiscDeployerWeapon Deployer { get; set; }

	public Transform FollowObject { get; set; }

	public Vector3 FloatOffset
	{
		get
		{
			return _floatOffset;
		}
	}

	public override void Start()
	{
		base.Start();
		_halfBobHeight = _floatBobHeight / 2f;
		_bobVelocity = _maxBobSpeedPerSecond;
		ConstantDamageSource componentInChildren = GetComponentInChildren<ConstantDamageSource>();
		if (componentInChildren != null)
		{
			Physics.IgnoreCollision(componentInChildren.collider, base.collider);
		}
		if (IsRemote)
		{
			if (base.rigidbody != null)
			{
				UnityEngine.Object.Destroy(base.rigidbody);
			}
			if (base.collider != null)
			{
				UnityEngine.Object.Destroy(base.collider);
			}
		}
	}

	public override void Update()
	{
		base.Update();
		FollowTarget();
		BobUpOrDownForFrame();
		if (Owner != null && Owner.IsDead)
		{
			Expired();
		}
	}

	protected int GetSpawnIndex()
	{
		int spawnIndex = _spawnIndex;
		_spawnIndex = ((_spawnIndex + 1 < _spawnPoints.Length) ? (_spawnIndex + 1) : 0);
		return spawnIndex;
	}

	private void FollowTarget()
	{
		if (FollowObject != null)
		{
			Vector3 to = FollowObject.transform.position + FollowObject.transform.right * _floatOffset.x;
			to += FollowObject.transform.up * _floatOffset.y;
			to += FollowObject.transform.forward * _floatOffset.z;
			base.transform.position = Vector3.Lerp(base.transform.position, to, 0.1f);
		}
	}

	private void BobUpOrDownForFrame()
	{
		Vector3 position = base.transform.position;
		position.y += Mathf.Sin((float)Math.PI * Time.time) * _maxBobSpeedPerSecond * Time.deltaTime;
		base.transform.position = position;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!IsRemote)
		{
			PlayerController component = other.gameObject.GetComponent<PlayerController>();
			if (component == null)
			{
				Expired();
			}
		}
	}

	public void Explode()
	{
		if (_explosionEffect != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(_explosionEffect, base.transform.position, Quaternion.identity) as GameObject;
		}
		if (!IsRemote && Owner != null && Owner.NetSync != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable[(byte)0] = Owner.OwnerID;
			hashtable[(byte)1] = DiscIndex;
			Owner.NetSync.SetAction(39, hashtable);
		}
	}

	protected override void Expired()
	{
		Explode();
		if (Deployer != null)
		{
			Deployer.OnDiscDestroyed();
		}
		base.Expired();
	}

	public void OnDestroyDisc()
	{
		Expired();
	}
}
