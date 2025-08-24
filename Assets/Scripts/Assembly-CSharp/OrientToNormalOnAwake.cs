using UnityEngine;

public class OrientToNormalOnAwake : MonoBehaviour
{
	[SerializeField]
	private LayerMask _layerMask;

	private Vector3 _raycastOffset = new Vector3(0f, 100f, 0f);

	private float _raycastMaxDistance = 4000f;

	private void Start()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position + _raycastOffset, Vector3.down, out hitInfo, _raycastMaxDistance, _layerMask))
		{
			SetForwardFromNormal(base.transform.forward, GetRealNormal(hitInfo.point, hitInfo.normal, hitInfo.collider));
		}
	}

	private void SetForwardFromNormal(Vector3 velocity, Vector3 contactNormal)
	{
		Vector3 vector = Vector3.Project(velocity, contactNormal);
		velocity -= vector;
		if (velocity != Vector3.zero)
		{
			base.transform.forward = velocity.normalized;
		}
		else
		{
			base.transform.forward = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z);
		}
	}

	private Vector3 GetRealNormal(Vector3 contactPoint, Vector3 contactNormal, Collider otherCollider)
	{
		float distance = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		RaycastHit[] array = Physics.RaycastAll(position, contactPoint - position, distance, 1 << otherCollider.gameObject.layer);
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (raycastHit.collider == otherCollider)
			{
				return raycastHit.normal;
			}
		}
		return contactNormal;
	}
}
