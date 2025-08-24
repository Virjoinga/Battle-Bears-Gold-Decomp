using UnityEngine;

public class AnimationSpeed : MonoBehaviour
{
	public float inSpeed;

	public float outSpeed;

	private void Start()
	{
		base.GetComponent<Animation>()["in"].speed = inSpeed;
		base.GetComponent<Animation>()["out"].speed = outSpeed;
	}

	private void Update()
	{
	}
}
