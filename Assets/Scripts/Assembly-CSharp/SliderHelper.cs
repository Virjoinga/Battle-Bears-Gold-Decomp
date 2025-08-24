using System;
using UnityEngine;

public class SliderHelper : MonoBehaviour
{
	private Collider _collider;

	private float _currentPercentage;

	private Action<float> _operationMethod;

	[SerializeField]
	protected Transform _indicator;

	[SerializeField]
	protected TextMesh _percentageText;

	[SerializeField]
	protected float _percentNearEdge = 0.1f;

	public virtual string PercentageText
	{
		get
		{
			return _percentageText.text;
		}
		protected set
		{
			_percentageText.text = value;
		}
	}

	private void Awake()
	{
		_collider = base.collider;
		_currentPercentage = _collider.bounds.size.x * _percentNearEdge;
	}

	public void SetOperationMethod(Action<float> opMethod)
	{
		_operationMethod = opMethod;
	}

	public void SetIndicatorToPercent(float percentage)
	{
		float a = _collider.bounds.min.x + _currentPercentage + percentage * (_collider.bounds.size.x - _currentPercentage * 2f);
		float a2 = Mathf.Max(a, _collider.bounds.min.x + _currentPercentage);
		a2 = Mathf.Min(a2, _collider.bounds.min.x + _collider.bounds.size.x - _currentPercentage);
		_indicator.position = new Vector3(a2, _indicator.transform.position.y, _indicator.transform.position.z);
		SetPercentText(_percentageText, percentage);
	}

	public virtual void SetIndicatorToWorldPos(float hitPoint)
	{
		float num = (hitPoint - _collider.bounds.min.x - _currentPercentage) / (_collider.bounds.size.x - _currentPercentage * 2f);
		float a = Mathf.Max(hitPoint, _collider.bounds.min.x + _currentPercentage);
		a = Mathf.Min(a, _collider.bounds.min.x + _collider.bounds.size.x - _currentPercentage);
		_indicator.position = new Vector3(a, _indicator.position.y, _indicator.position.z);
		SetPercentText(_percentageText, num);
		if (_operationMethod != null)
		{
			_operationMethod(num);
		}
	}

	public virtual void SetPercentText(TextMesh textMesh, float percentage)
	{
		percentage *= 100f;
		if (percentage < 0f)
		{
			percentage = 0f;
		}
		if (percentage > 100f)
		{
			percentage = 100f;
		}
		textMesh.text = (int)percentage + "%";
	}
}
