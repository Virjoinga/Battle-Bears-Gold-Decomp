using UnityEngine;

public class HUDCamera : MonoBehaviour
{
	[SerializeField]
	private float _aspectRatio = 1.5f;

	[SerializeField]
	private float _orthoSize = 160f;

	private void Update()
	{
		base.camera.orthographic = true;
		base.camera.orthographicSize = _orthoSize;
		base.camera.aspect = _aspectRatio;
	}
}
