using UnityEngine;

public class CODAimAccelerator : IAimAccelerator
{
	private float _rampSpeedPerSecond = 5f;

	private float _defaultCoefficient = 0.05f;

	private float _coefficient = 0.05f;

	private int _decreaseCoefficientThreshold = 2;

	private int _currentThreshold;

	public float CalculateSensitivityCoefficient(Vector2 aimingValues, float deltaTime)
	{
		if (aimingValues.x == 0f && aimingValues.y == 0f)
		{
			if (_currentThreshold > _decreaseCoefficientThreshold)
			{
				_coefficient = _defaultCoefficient;
			}
			else
			{
				_currentThreshold++;
			}
		}
		else
		{
			_coefficient += deltaTime * _rampSpeedPerSecond;
			_coefficient = Mathf.Clamp(_coefficient, _defaultCoefficient, 1f);
			_currentThreshold = 0;
		}
		return _coefficient;
	}
}
