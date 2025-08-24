using System.Collections;
using UnityEngine;

public class DelayedCollision : MonoBehaviour
{
	public float duration = 0.2f;

	public void Awake()
	{
		base.collider.enabled = false;
		StartCoroutine(delayedCollision());
	}

	private IEnumerator delayedCollision()
	{
		yield return new WaitForSeconds(duration);
		base.collider.enabled = true;
	}
}
