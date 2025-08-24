using UnityEngine;

public class ScaleOverTime : MonoBehaviour
{
	[SerializeField]
	private float _start;

	[SerializeField]
	private float _end;

	[SerializeField]
	private float _scaleTime;

	private float _startTime;

	public float StartScale
	{
		get
		{
			return _start;
		}
		set
		{
			_startTime = value;
		}
	}

	public float EndScale
	{
		get
		{
			return _end;
		}
		set
		{
			_end = value;
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
		float num = Mathf.Lerp(_start, _end, (Time.fixedTime - _startTime) / _scaleTime);
		base.transform.localScale = new Vector3(num, num, num);
		if (num == _end)
		{
			if (SetGameObjectInActiveAfterScale)
			{
				base.gameObject.SetActive(false);
			}
			Object.Destroy(this);
		}
	}
}
