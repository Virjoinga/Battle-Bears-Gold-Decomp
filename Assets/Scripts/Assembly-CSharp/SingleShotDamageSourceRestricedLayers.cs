using UnityEngine;

public class SingleShotDamageSourceRestricedLayers : SingleShotDamageSource
{
	public LayerMask layersToCollideWith;

	protected override void handleCollision(GameObject target)
	{
		if (((int)layersToCollideWith & (1 << target.layer)) != 0)
		{
			base.handleCollision(target);
		}
	}
}
