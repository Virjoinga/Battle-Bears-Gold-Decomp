using System.Collections;
using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
	public float initialDelayTime;

	public string animationName;

	public float extraStayTime;

	private void Awake()
	{
		StartCoroutine(destroyBehaviour());
	}

	private IEnumerator destroyBehaviour()
	{
		yield return new WaitForSeconds(initialDelayTime);
		if (animationName == string.Empty)
		{
			animationName = "idle";
		}
		Animation myAnimation = base.animation;
		myAnimation.Play(animationName);
		yield return new WaitForSeconds(myAnimation[animationName].length + extraStayTime);
		Object.Destroy(base.gameObject);
	}
}
