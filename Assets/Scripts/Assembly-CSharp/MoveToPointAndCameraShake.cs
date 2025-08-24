using UnityEngine;

public class MoveToPointAndCameraShake : MonoBehaviour
{
	[SerializeField]
	protected bool shakeScreen = true;

	[SerializeField]
	protected float shakeDuration = 1f;

	[SerializeField]
	protected float shakeStrength = 4f;

	[SerializeField]
	protected float explosionShakeRadius = 4000f;

	[SerializeField]
	protected float shakeFunctionExponent = 3f;

	public GameObject ObjectToSpawnAtPoint { get; set; }

	public Vector3 Destination { get; set; }

	public float Speed { get; set; }

	private void Update()
	{
		base.transform.position = Vector3.MoveTowards(base.transform.position, Destination, Speed * Time.deltaTime);
		if (base.transform.position == Destination)
		{
			DoCameraShake();
			if (ObjectToSpawnAtPoint != null)
			{
				Object.Instantiate(ObjectToSpawnAtPoint, Destination, Quaternion.identity);
			}
			Object.Destroy(this);
		}
	}

	protected void DoCameraShake()
	{
		if (shakeScreen && Camera.main != null)
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
