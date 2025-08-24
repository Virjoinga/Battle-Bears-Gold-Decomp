using UnityEngine;

public class DelayedDestroyHomingMissile : HomingMissile
{
	[SerializeField]
	private float _destroyDelay = 3f;

	[SerializeField]
	private bool _stopInPlaceOnExplode;

	public override void Explode(GameObject objectHit)
	{
		base.Explode(objectHit);
		if (_stopInPlaceOnExplode && base.rigidbody != null)
		{
			base.rigidbody.useGravity = false;
			base.rigidbody.isKinematic = true;
			base.rigidbody.detectCollisions = false;
			base.rigidbody.Sleep();
		}
	}

	public override void TryDestroy()
	{
		Object.Destroy(base.gameObject, _destroyDelay);
	}
}
