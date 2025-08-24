using UnityEngine;

public class DontGoThroughThings : MonoBehaviour
{
	public LayerMask layerMask;

	public float skinWidth = 0.1f;

	private float minimumExtent;

	private float partialExtent;

	private float sqrMinimumExtent;

	private Vector3 previousPosition;

	private Rigidbody myRigidbody;

	private void Awake()
	{
		myRigidbody = base.rigidbody;
		previousPosition = myRigidbody.position;
		minimumExtent = Mathf.Min(Mathf.Min(base.collider.bounds.extents.x, base.collider.bounds.extents.y), base.collider.bounds.extents.z);
		partialExtent = minimumExtent * (1f - skinWidth);
		sqrMinimumExtent = minimumExtent * minimumExtent;
	}

	private void LateUpdate()
	{
		Vector3 vector = myRigidbody.position - previousPosition;
		float sqrMagnitude = vector.sqrMagnitude;
		if (sqrMagnitude > sqrMinimumExtent)
		{
			float num = Mathf.Sqrt(sqrMagnitude);
			RaycastHit hitInfo;
			if (Physics.Raycast(previousPosition, vector, out hitInfo, num, layerMask.value))
			{
				base.transform.position = hitInfo.point - vector / num * partialExtent;
			}
		}
		previousPosition = myRigidbody.position;
	}
}
