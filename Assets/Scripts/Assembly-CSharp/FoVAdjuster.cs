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
		_initialPos = base.GetComponent<Camera>().transform.localPosition;
		_scale = base.GetComponent<Camera>().transform.localScale;
		base.GetComponent<Camera>().fov = _baseFoV;
	}

	private void Update()
	{
		if (_forceAspect)
		{
			base.GetComponent<Camera>().aspect = _targetAspectRatio;
		}
		base.transform.localPosition = _initialPos + _offset;
		base.transform.localPosition = _initialScale + _scale;
	}
}
