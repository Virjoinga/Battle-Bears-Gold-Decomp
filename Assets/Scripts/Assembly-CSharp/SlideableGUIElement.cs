using UnityEngine;

public class SlideableGUIElement : DecorativeGUIElement
{
	private float _totalSlideTime;

	private float _startSlideTime;

	private float _currentSlideTime;

	private float _desiredSlidePercent;

	private float _slidePercent;

	private float _slideRatePerSecond = 0.05f;

	public float DesiredSlidePercent
	{
		get
		{
			return _desiredSlidePercent;
		}
		set
		{
			_desiredSlidePercent = value;
			_startSlideTime = Time.fixedTime;
			_currentSlideTime = _startSlideTime;
			_totalSlideTime = Mathf.Abs(_slidePercent - _desiredSlidePercent) / _slideRatePerSecond;
		}
	}

	public float SlidePercent
	{
		get
		{
			return _slidePercent;
		}
		set
		{
			_slidePercent = value;
		}
	}

	public float SlideRatePerSecond
	{
		get
		{
			return _slideRatePerSecond;
		}
		set
		{
			_slideRatePerSecond = value;
		}
	}

	public override Rect CreateDrawRect()
	{
		if (_slidePercent != _desiredSlidePercent)
		{
			_currentSlideTime += Time.smoothDeltaTime;
			_slidePercent = Mathf.Lerp(_slidePercent, _desiredSlidePercent, (_currentSlideTime - _startSlideTime) / _totalSlideTime);
		}
		_textureCoordinates = new Rect(0f, 0f, _slidePercent, 1f);
		return new Rect(base.PositionX, base.PositionY, base.Width * _slidePercent, base.Height);
	}
}
