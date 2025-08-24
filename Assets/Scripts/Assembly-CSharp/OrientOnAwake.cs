using UnityEngine;

public class OrientOnAwake : MonoBehaviour
{
	[SerializeField]
	private Vector3 orientation;

	private void Awake()
	{
		base.transform.eulerAngles = orientation;
	}
}
