using UnityEngine;

public class DynamicJoystick : MonoBehaviour
{
	public int touchID = -1;

	public Camera joystickCamera;

	public float inputRadius = 10f;

	public Vector3 currentDir;

	public float currentMagnitude;

	private Vector3 startPoint = Vector3.zero;

	private Vector3 currentHitpoint;

	private Joystick moveJoystick;

	public PlayerController playerController;

	public LayerMask layerMask;

	private void Awake()
	{
		moveJoystick = Object.FindObjectOfType(typeof(Joystick)) as Joystick;
	}

	private void Update()
	{
		if (touchID != -1 || !(playerController != null))
		{
			return;
		}
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (moveJoystick.getTouchID() != touch.fingerId && (touch.phase != 0 || !moveJoystick.TouchIsInJoystickBounds(touch)) && touch.phase == TouchPhase.Began)
			{
				Ray ray = joystickCamera.ScreenPointToRay(touch.position);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray.origin, ray.direction, out hitInfo, 2000f, layerMask) && hitInfo.transform.gameObject == base.gameObject)
				{
					touchID = touch.fingerId;
					startPoint = hitInfo.point;
					currentHitpoint = startPoint;
					break;
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (Input.touchCount == 0)
		{
			touchID = -1;
			currentDir = Vector3.zero;
			currentMagnitude = 0f;
		}
		else if (touchID != -1)
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				if (touch.fingerId != touchID)
				{
					continue;
				}
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					touchID = -1;
					currentDir = Vector3.zero;
					currentMagnitude = 0f;
					break;
				}
				if (touch.phase == TouchPhase.Moved)
				{
					Ray ray = joystickCamera.ScreenPointToRay(touch.position);
					RaycastHit hitInfo;
					if (Physics.Raycast(ray.origin, ray.direction, out hitInfo, 2000f, layerMask))
					{
						currentHitpoint = hitInfo.point;
						calculateDirection();
					}
					break;
				}
			}
		}
		else
		{
			currentDir = Vector3.zero;
			currentMagnitude = 0f;
		}
	}

	private void calculateDirection()
	{
		Vector3 vector = currentHitpoint - startPoint;
		if (vector.magnitude > inputRadius)
		{
			vector.Normalize();
			vector *= inputRadius;
		}
		currentDir = vector.normalized;
		currentDir.z = currentDir.y;
		currentDir.y = 0f;
		currentMagnitude = vector.magnitude / inputRadius * (0.5f + 2f * Preferences.Instance.Sensitivity);
	}

	private void OnDrawGizmos()
	{
		if (startPoint != Vector3.zero)
		{
			Gizmos.DrawWireSphere(startPoint, inputRadius);
			Gizmos.DrawLine(startPoint, currentHitpoint);
		}
	}

	public int getTouchID()
	{
		return touchID;
	}
}
