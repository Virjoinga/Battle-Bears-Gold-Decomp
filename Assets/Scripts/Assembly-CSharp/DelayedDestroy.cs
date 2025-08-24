using System.Collections;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{
	public float delay = 10f;

	private void Start()
	{
		StartCoroutine(delayedDestroy());
	}

	private IEnumerator delayedDestroy()
	{
		yield return new WaitForSeconds(delay);
		Object.Destroy(base.gameObject);
	}
}
