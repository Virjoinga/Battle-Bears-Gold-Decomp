using UnityEngine;

public class LOSSimpleConstantDamageSource : SimpleConstantDamageSource
{
	[SerializeField]
	private LayerMask _layersToHit;

	protected override void dealDamage(GameObject target)
	{
		if (checkForActualHit(target))
		{
			base.dealDamage(target);
		}
	}

	protected bool checkForActualHit(GameObject target)
	{
		bool result = false;
		if (target != null && target.GetComponent<Collider>() != null && base.transform != null)
		{
			Bounds bounds = target.GetComponent<Collider>().bounds;
			Vector3[] array = new Vector3[2]
			{
				new Vector3(bounds.center.x, bounds.max.y, bounds.center.z),
				new Vector3(bounds.center.x, bounds.min.y, bounds.center.z)
			};
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 vector = array[i] - base.transform.position;
				RaycastHit hitInfo;
				if (!Physics.Raycast(base.transform.position, vector.normalized, out hitInfo, vector.magnitude, _layersToHit))
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}
}
