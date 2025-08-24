using UnityEngine;

public class FoVAdjuster : MonoBehaviour
{
	[SerializeField]
	private bool _forceAspect = true;

	[SerializeField]
	private float _baseFoV;

	[SerializeField]
	private float _targetAspectRatio;

	[SerializeField]
	private Vector3 _offset;

	[SerializeField]
	private Vector3 _scale;

	private Vector3 _initialPos;

	private Vector3 _initialScale;

	private void Start()
	{
		_initialPos = base.camera.transform.localPosition;
		_scale = base.camera.transform.localScale;
		base.camera.fov = _baseFoV;
	}

	private void Update()
	{
		if (_forceAspect)
		{
			base.camera.aspect = _targetAspectRatio;
		}
		base.transform.localPosition = _initialPos + _offset;
		base.transform.localPosition = _initialScale + _scale;
	}
}
