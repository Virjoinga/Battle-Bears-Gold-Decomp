using System.Collections.Generic;
using UnityEngine;

public class BounceLazer : WeaponBase
{
	private const int WALL_LAYER = 9;

	private const int PLAYER_LAYER = 10;

	private const int RAYCASTABLE_WALL_LAYER = 26;

	private const int DESTRUCTABLE_LAYER = 28;

	private const int CEILING_LAYER = 24;

	[SerializeField]
	private LineRenderer _beam;

	[SerializeField]
	private float _damage;

	[SerializeField]
	protected float _range;

	[SerializeField]
	protected LayerMask _layersToHit = LayersToHitFromConstants();

	[SerializeField]
	private Transform[] _spawnPoints;

	private bool _isFiring;

	protected Transform _bodyRotator;

	private int _bounces = 4;

	public override void ConfigureWeapon(Item item)
	{
		base.ConfigureWeapon(item);
		item.UpdateProperty("damage", ref _damage, base.EquipmentNames);
		item.UpdateProperty("range", ref _range, base.EquipmentNames);
		item.UpdateProperty("bounces", ref _bounces, base.EquipmentNames);
	}

	private static LayerMask LayersToHitFromConstants()
	{
		return 352323072;
	}

	public override void BeginConstantFireEffects()
	{
		base.BeginConstantFireEffects();
		base.GetComponent<AudioSource>().Play();
		_isFiring = true;
	}

	public override void EndConstantFireEffects()
	{
		base.EndConstantFireEffects();
		base.GetComponent<AudioSource>().Stop();
		_isFiring = false;
	}

	private void Update()
	{
		if (_isFiring)
		{
			FireLazer();
		}
		else
		{
			HideLazer();
		}
	}

	private void FireLazer()
	{
		RaycastHit[] lazerHits = RaycastHitsFromBouncingLazer();
		DisplayLazerLine(lazerHits);
		TryDealDamageToHits(lazerHits);
	}

	private void HideLazer()
	{
		_beam.enabled = false;
	}

	protected RaycastHit[] RaycastHitsFromBouncingLazer()
	{
		List<RaycastHit> list = new List<RaycastHit>();
		RaycastHit firstHit = HitFromRay(RayFromGun(), _range);
		if (firstHit.collider != null)
		{
			foreach (RaycastHit item in AllRaycastHitsForBounces(firstHit))
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	protected RaycastHit HitFromRay(Ray ray, float range)
	{
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray.origin, ray.direction, out hitInfo, range, _layersToHit))
		{
			hitInfo.point = EndPointOfRay(ray, range);
		}
		return hitInfo;
	}

	private Ray RayFromGun()
	{
		if (_bodyRotator == null && myTransform != null)
		{
			_bodyRotator = (myTransform.root.GetComponentInChildren(typeof(PlayerController)) as PlayerController).bodyRotator;
		}
		return (!isRemote) ? Camera.main.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f)) : new Ray(_spawnPoints[0].position, _bodyRotator.forward);
	}

	private List<RaycastHit> AllRaycastHitsForBounces(RaycastHit firstHit)
	{
		List<RaycastHit> list = new List<RaycastHit>();
		list.Add(firstHit);
		for (int i = 0; i < _bounces && ShouldBounceOffHitPoint(list, i); i++)
		{
			Ray ray = new Ray(list[i].point, ReflectionFromHitNormal(list, i));
			list.Add(HitFromRay(ray, _range - TotalDistanceFromHits(list)));
		}
		return list;
	}

	private bool ShouldBounceOffHitPoint(List<RaycastHit> hits, int index)
	{
		DamageReceiver damageReceiver = null;
		if (hits[index].transform != null)
		{
			damageReceiver = hits[index].transform.GetComponent<DamageReceiver>();
		}
		return damageReceiver == null && TotalDistanceFromHits(hits) < _range;
	}

	private float TotalDistanceFromHits(List<RaycastHit> hits)
	{
		float num = 0f;
		foreach (RaycastHit hit in hits)
		{
			num += hit.distance;
		}
		return num;
	}

	private Vector3 ReflectionFromHitNormal(List<RaycastHit> hits, int index)
	{
		Vector3 inDirection = ((index <= 0) ? (hits[index].point - _spawnPoints[0].position) : (hits[index].point - hits[index - 1].point));
		return Vector3.Reflect(inDirection, hits[index].normal);
	}

	private void DisplayLazerLine(RaycastHit[] lazerHits)
	{
		List<Vector3> list = PointsFromRaycastHits(lazerHits);
		_beam.SetVertexCount(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			_beam.SetPosition(i, list[i]);
		}
		_beam.enabled = true;
	}

	private List<Vector3> PointsFromRaycastHits(RaycastHit[] lazerHits)
	{
		List<Vector3> list = new List<Vector3>();
		list.Add(_spawnPoints[0].position);
		List<Vector3> list2 = list;
		if (lazerHits != null && lazerHits.Length > 0)
		{
			foreach (RaycastHit raycastHit in lazerHits)
			{
				list2.Add(raycastHit.point);
			}
		}
		else
		{
			list2.Add(EndPointOfRay(RayFromGun(), _range));
		}
		return list2;
	}

	private Vector3 EndPointOfRay(Ray ray, float range)
	{
		return ray.origin + ray.direction * range;
	}

	private void TryDealDamageToHits(RaycastHit[] lazerHits)
	{
		if (lazerHits == null || lazerHits.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < lazerHits.Length; i++)
		{
			RaycastHit hit = lazerHits[i];
			if (hit.transform != null)
			{
				TryDealDamage(hit);
			}
		}
	}

	private void TryDealDamage(RaycastHit hit)
	{
		DamageReceiver component = hit.transform.GetComponent<DamageReceiver>();
		if (component != null && !component.isInvincible)
		{
			PlayerController component2 = component.transform.root.GetComponent<PlayerController>();
			if (component2 != base.playerController && component2.Team != base.playerController.Team)
			{
				OnHit(component);
			}
		}
	}

	protected virtual void OnHit(DamageReceiver dmgReceiver)
	{
		OnDealDirectDamage(dmgReceiver, _damage * base.playerController.DamageMultiplier * Time.deltaTime);
	}
}
