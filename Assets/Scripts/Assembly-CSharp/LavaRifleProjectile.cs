using UnityEngine;

public class LavaRifleProjectile : DeployableProjectile
{
	[SerializeField]
	private LayerMask _hitMask;

	private Vector3 _lastPosition;

	private Vector3 _twoPositionsAgo;

	protected override Ray GetRay(Collision collisionInfo, float distance)
	{
		Vector3 vector = ((!(_twoPositionsAgo != default(Vector3))) ? collisionInfo.contacts[0].normal : (_lastPosition - _twoPositionsAgo).normalized);
		Vector3 origin = collisionInfo.contacts[0].point - distance * vector;
		return new Ray(origin, vector);
	}

	protected override float GetRaycastDistance()
	{
		return (!(_twoPositionsAgo != default(Vector3))) ? base.GetRaycastDistance() : (Vector3.Distance(_twoPositionsAgo, _lastPosition) + 1f);
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (!_isBeingDeployed && !_isBeingDestroyed)
		{
			CheckForMissedCollisions();
			_twoPositionsAgo = _lastPosition;
			_lastPosition = base.transform.position;
		}
	}

	private void CheckForMissedCollisions()
	{
		if (!(_lastPosition == default(Vector3)))
		{
			Vector3 lastPosition = _lastPosition;
			Vector3 direction = base.transform.position - _lastPosition;
			RaycastHit hitInfo;
			if (Physics.Raycast(lastPosition, direction, out hitInfo, Vector3.Distance(_lastPosition, base.transform.position), _hitMask) && !TryDestroyByShieldOrCurtain(hitInfo.collider) && !HitIgnoredPlayerOrMGSBox(hitInfo.transform, hitInfo.collider))
			{
				_isBeingDeployed = true;
				DeployOrientedToNormal(hitInfo);
			}
		}
	}
}
