using UnityEngine;

public class SimpleTouchRotator : MonoBehaviour
{
	private Collider myCollider;

	private Transform myTransform;

	private int touchID = -1;

	private float currentRot;

	private float targetRot = 180f;

	private bool returnToZero;

	public AudioClip[] pukeSounds;

	private float nextAllowedPukeTime;

	private void Awake()
	{
		myCollider = base.GetComponent<Collider>();
		myTransform = base.transform;
	}

	private void Start()
	{
		currentRot = myTransform.localEulerAngles.y;
	}

	private void Update()
	{
		bool flag = false;
		for (int i = 0; i < Input.touches.Length; i++)
		{
			Touch touch = Input.touches[i];
			Ray ray = Camera.main.ScreenPointToRay(touch.position);
			RaycastHit hitInfo;
			if (myCollider.Raycast(ray, out hitInfo, 2000f))
			{
				if (touchID == -1)
				{
					touchID = touch.fingerId;
				}
				else if (touchID == touch.fingerId)
				{
					targetRot -= touch.deltaPosition.x * 2.5f / ((!BBRQuality.HighRes) ? 1f : 2f);
				}
				flag = true;
			}
		}
		if (!flag)
		{
			touchID = -1;
		}
	}

	public void OnForceReturnZero()
	{
		returnToZero = true;
		targetRot %= 360f;
		currentRot %= 360f;
	}

	public void OnSetTarget(float target)
	{
		targetRot = Mathf.Round(targetRot / target) * target - (target - 360f);
	}

	private void LateUpdate()
	{
		if (!returnToZero)
		{
			if (Mathf.Abs(currentRot - targetRot) > 1350f && Time.time > nextAllowedPukeTime && pukeSounds.Length > 0)
			{
				AudioClip audioClip = pukeSounds[Random.Range(0, pukeSounds.Length)];
				base.GetComponent<AudioSource>().PlayOneShot(audioClip);
				nextAllowedPukeTime = Time.time + audioClip.length + 0.5f;
			}
			currentRot = Mathf.Lerp(currentRot, targetRot, Time.deltaTime * 1.5f);
		}
		else
		{
			currentRot = Mathf.Lerp(currentRot, 0f, Time.deltaTime * 2f);
		}
		myTransform.localEulerAngles = new Vector3(0f, currentRot, 0f);
	}
}
