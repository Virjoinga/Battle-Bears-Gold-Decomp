using System.Collections;
using UnityEngine;

public class SynchronizedAnimation : MonoBehaviour
{
	public string animationName;

	public float delayAtStart = 3f;

	public void OnStartup(float delay)
	{
		StartCoroutine(delayedStart(delay));
	}

	private IEnumerator delayedStart(float delay)
	{
		yield return new WaitForSeconds(delayAtStart - delay);
		if (base.GetComponent<Animation>()[animationName] != null)
		{
			base.GetComponent<Animation>().Play(animationName);
		}
	}
}
