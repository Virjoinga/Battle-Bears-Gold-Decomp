using UnityEngine;

public class ScaleOverTimeXYZ : MonoBehaviour
{
	[SerializeField]
	private Vector3 _startScales;

	[SerializeField]
	private Vector3 _endScales;

	[SerializeField]
	private float _scaleTime;

	private float _startTime;

	public Vector3 StartScale
	{
		get
		{
			return _startScales;
		}
		set
		{
			_startScales = value;
		}
	}

	public Vector3 EndScale
	{
		get
		{
			return _endScales;
		}
		set
		{
			_endScales = value;
		}
	}

	public float ScaleTime
	{
		get
		{
			return _scaleTime;
		}
		set
		{
			_scaleTime = value;
		}
	}

	public bool SetGameObjectInActiveAfterScale { get; set; }

	private void Start()
	{
		_startTime = Time.fixedTime;
	}

	private void Update()
	{
		Vector3 vector = new Vector3(Mathf.Lerp(_startScales.x, _endScales.x, (Time.fixedTime - _startTime) / _scaleTime), Mathf.Lerp(_startScales.y, _endScales.y, (Time.fixedTime - _startTime) / _scaleTime), Mathf.Lerp(_startScales.z, _endScales.z, (Time.fixedTime - _startTime) / _scaleTime));
		base.transform.localScale = vector;
		if (vector == _endScales)
		{
			if (SetGameObjectInActiveAfterScale)
			{
				base.gameObject.SetActive(false);
				base.transform.localScale = _startScales;
			}
			Object.Destroy(this);
		}
	}
}
