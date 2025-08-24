using UnityEngine;

public class MoveToPointAndDestroy : MonoBehaviour
{
	private float _speed;

	public Vector3 Destination { get; set; }

	public float Acceleration { get; set; }

	public float MaxSpeed { get; set; }

	private void Update()
	{
		if (_speed != MaxSpeed)
		{
			_speed += Acceleration * Time.deltaTime;
			if (_speed > MaxSpeed)
			{
				_speed = MaxSpeed;
			}
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, Destination, _speed * Time.deltaTime);
		if (base.transform.position.AlmostEquals(Destination, 100f))
		{
			Object.Destroy(base.gameObject);
		}
	}
}
