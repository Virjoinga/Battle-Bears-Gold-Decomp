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
		if (_stopInPlaceOnExplode && base.GetComponent<Rigidbody>() != null)
		{
			base.GetComponent<Rigidbody>().useGravity = false;
			base.GetComponent<Rigidbody>().isKinematic = true;
			base.GetComponent<Rigidbody>().detectCollisions = false;
			base.GetComponent<Rigidbody>().Sleep();
		}
	}

	public override void TryDestroy()
	{
		Object.Destroy(base.gameObject, _destroyDelay);
	}
}
