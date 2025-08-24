using UnityEngine;

public class HUDCamera : MonoBehaviour
{
	[SerializeField]
	private float _aspectRatio = 1.5f;

	[SerializeField]
	private float _orthoSize = 160f;

	private void Update()
	{
		base.GetComponent<Camera>().orthographic = true;
		base.GetComponent<Camera>().orthographicSize = _orthoSize;
		base.GetComponent<Camera>().aspect = _aspectRatio;
	}
}
