using UnityEngine;

public class IPadAdjuster : MonoBehaviour
{
	public float ipadFOV;

	public Vector3 offset;

	public Vector3 scale;

	public float ipadOrtho;

	public Rect normalizedViewPortRect = new Rect(0f, 0f, 1f, 1f);

	private Vector3 initialPosition;

	private float initialPOV;

	private float initialOrtho;

	private Rect initialRect;

	private Vector3 initialScale;

	private Camera myCamera;

	private Transform myTransform;

	private void Awake()
	{
		myCamera = base.camera;
		myTransform = base.transform;
		init();
	}

	private void init()
	{
		initialPosition = myTransform.localPosition;
		initialScale = myTransform.localScale;
		if (myCamera != null)
		{
			initialPOV = myCamera.fieldOfView;
			initialOrtho = myCamera.orthographicSize;
			initialRect = myCamera.rect;
		}
		if (normalizedViewPortRect.width <= 0f || normalizedViewPortRect.height <= 0f)
		{
			normalizedViewPortRect = new Rect(0f, 0f, 1f, 1f);
		}
	}
}
