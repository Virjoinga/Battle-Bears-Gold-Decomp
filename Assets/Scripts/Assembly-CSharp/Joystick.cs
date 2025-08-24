using UnityEngine;

public class Joystick : MonoBehaviour
{
	private int touchID = -2;

	private Collider myCollider;

	private Transform myTransform;

	public Collider inputCollider;

	public Camera joystickCamera;

	private Transform indicator;

	public float inputRadius = 10f;

	public Vector3 currentDir;

	public float currentMagnitude;

	private DynamicJoystick aimJoystick;

	private float _mogaSensitivityMod = 0.3f;

	private void Awake()
	{
		myTransform = base.transform;
		myCollider = base.GetComponent<Collider>();
		indicator = myTransform.Find("indicator");
		aimJoystick = Object.FindObjectOfType(typeof(DynamicJoystick)) as DynamicJoystick;
	}

	private void Update()
	{
		if (Preferences.Instance.CurrentControlMode == ControlMode.defaultHud)
		{
			return;
		}
		if (Input.touchCount == 0)
		{
			touchID = -2;
			indicator.localPosition = Vector3.zero;
			currentDir = Vector3.zero;
			currentMagnitude = 0f;
		}
		else if (touchID == -2)
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				if (touch.phase == TouchPhase.Began && touch.fingerId != aimJoystick.touchID)
				{
					Ray ray = joystickCamera.ScreenPointToRay(touch.position);
					RaycastHit hitInfo;
					if (myCollider.Raycast(ray, out hitInfo, 2000f))
					{
						touchID = touch.fingerId;
						indicator.position = hitInfo.point;
					}
				}
			}
		}
		else
		{
			for (int j = 0; j < Input.touchCount; j++)
			{
				Touch touch2 = Input.GetTouch(j);
				if (touch2.fingerId != touchID)
				{
					continue;
				}
				if (touch2.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Canceled)
				{
					indicator.localPosition = Vector3.zero;
					touchID = -2;
				}
				else if (touch2.phase == TouchPhase.Moved)
				{
					Ray ray2 = joystickCamera.ScreenPointToRay(touch2.position);
					RaycastHit hitInfo2;
					if (inputCollider.Raycast(ray2, out hitInfo2, 2000f))
					{
						indicator.position = hitInfo2.point;
					}
				}
			}
		}
		updateIndicatorAndDirection();
	}

	public bool TouchIsInJoystickBounds(Touch touchToTest)
	{
		Ray ray = joystickCamera.ScreenPointToRay(touchToTest.position);
		RaycastHit hitInfo;
		if (myCollider.Raycast(ray, out hitInfo, 2000f))
		{
			return true;
		}
		return false;
	}

	private void updateIndicatorAndDirection()
	{
		Vector3 vector = indicator.position - myTransform.position;
		MogaController instance = MogaController.Instance;
		if (instance.connection == 1 && HUD.Instance.currentPauseMenu == null && HUD.Instance.currentTeamspeakOverlay == null && HUD.Instance.currentStatsOverlay == null)
		{
			vector.x = instance.axisX * _mogaSensitivityMod;
			vector.y = (0f - instance.axisY) * _mogaSensitivityMod;
		}
		if (vector.magnitude > inputRadius)
		{
			vector.Normalize();
			vector *= inputRadius;
			indicator.position = myTransform.position + vector;
		}
		currentDir = vector.normalized;
		currentDir.z = 0f;
		currentMagnitude = vector.magnitude / inputRadius;
	}

	private void OnDrawGizmosSelected()
	{
		if (myTransform != null && indicator != null)
		{
			Gizmos.DrawWireSphere(myTransform.position, inputRadius);
			Gizmos.DrawLine(myTransform.position, indicator.position);
		}
	}

	public int getTouchID()
	{
		return touchID;
	}
}
