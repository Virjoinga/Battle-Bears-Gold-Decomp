using UnityEngine;

public class RivetBoltEffect : MonoBehaviour
{
	private Transform _transform;

	private float _originalXRotation;

	private float _originalYRotation;

	private void Start()
	{
		_transform = base.transform;
		_originalXRotation = _transform.eulerAngles.x;
		_originalYRotation = _transform.eulerAngles.y;
	}

	private void Update()
	{
	}
}
