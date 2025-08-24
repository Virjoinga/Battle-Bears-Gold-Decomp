using UnityEngine;

public class CameraShakeWithAttenuation : MonoBehaviour
{
	[SerializeField]
	protected float shakeDuration = 1f;

	[SerializeField]
	protected float shakeStrength = 1f;

	[SerializeField]
	protected float explosionShakeRadius = 3000f;

	[SerializeField]
	protected float shakeFunctionExponent = 3f;

	private void Start()
	{
		DoCameraShake();
	}

	protected void DoCameraShake()
	{
		if (Camera.main != null)
		{
			float f = (explosionShakeRadius - Vector3.Distance(base.transform.position, Camera.main.transform.position)) / explosionShakeRadius;
			float num = Mathf.Pow(f, shakeFunctionExponent);
			if (num > 0f)
			{
				ShakeCamera shakeCamera = Camera.main.gameObject.AddComponent<ShakeCamera>();
				shakeCamera.shakeDuration = shakeDuration;
				shakeCamera.shakeStrength = shakeStrength * num;
			}
		}
	}
}
