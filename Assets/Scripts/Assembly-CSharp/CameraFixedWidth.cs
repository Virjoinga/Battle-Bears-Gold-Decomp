using UnityEngine;

public class CameraFixedWidth : MonoBehaviour
{
	public float aspectCreatedAt;

	public Camera cameraToAdjust;

	private float _initialSize;

	private void Start()
	{
		_initialSize = cameraToAdjust.orthographicSize;
	}

	private void Update()
	{
		cameraToAdjust.orthographicSize = _initialSize * (aspectCreatedAt / ((float)Screen.width / (float)Screen.height));
	}
}
