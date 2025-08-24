using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
	public Transform cameraTransform;

	private Transform myTransform;

	public float maxDistance;

	private void Awake()
	{
		myTransform = base.transform;
		if (cameraTransform == null && Camera.main != null)
		{
			cameraTransform = Camera.main.transform;
		}
	}

	private void LateUpdate()
	{
		if (cameraTransform == null && Camera.main != null)
		{
			cameraTransform = Camera.main.transform;
		}
		if (!(cameraTransform != null))
		{
			return;
		}
		Vector3 eulerAngles = cameraTransform.eulerAngles;
		eulerAngles.x = 0f - eulerAngles.x;
		eulerAngles.y -= 180f;
		eulerAngles.z = 0f - eulerAngles.z;
		myTransform.eulerAngles = eulerAngles;
		if (maxDistance > 0f)
		{
			float num = Vector3.Distance(myTransform.position, cameraTransform.position);
			if (num > maxDistance && myTransform.localScale.x > 0f)
			{
				myTransform.localScale = Vector3.zero;
			}
			else if (num <= maxDistance && myTransform.localScale.x == 0f)
			{
				myTransform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}
}
