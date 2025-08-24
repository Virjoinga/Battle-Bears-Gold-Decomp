using UnityEngine;

public class SnapToSurfaceOnAwake : MonoBehaviour
{
	[SerializeField]
	private Vector3 _direction = Vector3.down;

	[SerializeField]
	private float _maxDistanceToSnap;

	private void Awake()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position, _direction, out hitInfo, _maxDistanceToSnap))
		{
			base.transform.position = hitInfo.point;
		}
	}
}
