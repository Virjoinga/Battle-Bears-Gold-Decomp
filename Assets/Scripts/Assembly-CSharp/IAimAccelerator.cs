using UnityEngine;

public interface IAimAccelerator
{
	float CalculateSensitivityCoefficient(Vector2 aimingValues, float deltaTime);
}
