using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	private Vector3 normalPos;

	public Vector3 closestPos;

	private float closerDistance = 15f;

	public float rightWallDistance = 30f;

	private Transform myTransform;

	public LayerMask hitMask;

	private Transform parentTransform;

	private Vector3 origNormalPos;

	public Vector3 NormalPosition
	{
		get
		{
			return normalPos;
		}
		set
		{
			normalPos = value;
			calculateNewPosition();
		}
	}

	private void Awake()
	{
		myTransform = base.transform;
	}

	private void Start()
	{
		normalPos = myTransform.localPosition;
		origNormalPos = normalPos;
		parentTransform = myTransform.root;
	}

	public void OnResetNormalPosition()
	{
		normalPos = origNormalPos;
		calculateNewPosition();
	}

	private void calculateNewPosition()
	{
		myTransform.localPosition = closestPos;
		Vector3 position = myTransform.position;
		myTransform.localPosition = normalPos;
		Vector3 position2 = myTransform.position;
		RaycastHit hitInfo;
		if (Physics.Raycast(position, (position2 - position).normalized, out hitInfo, Vector3.Distance(position2, position), hitMask))
		{
			myTransform.position = hitInfo.point - (position2 - position).normalized * closerDistance * parentTransform.localScale.x;
		}
		else
		{
			myTransform.position = position2 - (position2 - position).normalized * closerDistance * parentTransform.localScale.x;
		}
		Vector3 position3 = myTransform.position;
		myTransform.Translate(Vector3.right * rightWallDistance);
		position = myTransform.position;
		if (Physics.Raycast(position3, (position - position3).normalized, out hitInfo, Vector3.Distance(position3, position), hitMask))
		{
			myTransform.position = hitInfo.point + (position3 - position).normalized * rightWallDistance * parentTransform.localScale.x;
		}
		else
		{
			myTransform.position = position3;
		}
	}

	private void LateUpdate()
	{
		calculateNewPosition();
	}
}
