using System.Collections;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
	public float shakeDuration = 0.8f;

	public float shakeStrength = 3f;

	private Transform cameraTransform;

	private void Awake()
	{
		if (Camera.main != null)
		{
			cameraTransform = Camera.main.transform;
		}
	}

	private void Start()
	{
		if (cameraTransform != null)
		{
			StartCoroutine(shakeCamera());
		}
	}

	private IEnumerator shakeCamera()
	{
		if (!(cameraTransform == null))
		{
			float shakeTimeLeft = shakeDuration;
			Vector3 rot = Vector3.zero;
			while (shakeTimeLeft > 0f)
			{
				rot.x = Random.Range(0f - shakeStrength, shakeStrength);
				rot.y = Random.Range(0f - shakeStrength, shakeStrength);
				cameraTransform.localEulerAngles += rot;
				yield return new WaitForSeconds(0.02f);
				shakeTimeLeft -= 0.02f;
				cameraTransform.localEulerAngles -= rot;
			}
			Object.Destroy(this);
		}
	}
}
