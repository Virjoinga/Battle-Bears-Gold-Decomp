using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
	public Vector3 rotation;

	private Transform myTransform;

	private void Awake()
	{
		myTransform = base.transform;
	}

	private void Update()
	{
		myTransform.Rotate(rotation * Time.deltaTime);
	}
}
