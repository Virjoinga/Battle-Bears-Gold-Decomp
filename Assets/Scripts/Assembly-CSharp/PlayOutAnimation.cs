using System.Collections;
using UnityEngine;

public class PlayOutAnimation : MonoBehaviour
{
	public string outAnimation;

	public float delayBeforeOut = 6f;

	public bool destroyAfterOut = true;

	private Animation myAnimation;

	private void Awake()
	{
		myAnimation = base.GetComponent<Animation>();
	}

	private void Start()
	{
		StartCoroutine(delayedOutAnimation());
	}

	private IEnumerator delayedOutAnimation()
	{
		yield return new WaitForSeconds(delayBeforeOut);
		myAnimation.Play(outAnimation);
		yield return new WaitForSeconds(myAnimation[outAnimation].length);
		if (destroyAfterOut)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
