using UnityEngine;

public class AnimationSpeed : MonoBehaviour
{
	public float inSpeed;

	public float outSpeed;

	private void Start()
	{
		base.animation["in"].speed = inSpeed;
		base.animation["out"].speed = outSpeed;
	}

	private void Update()
	{
	}
}
