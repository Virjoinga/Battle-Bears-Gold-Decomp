using System.Collections;
using UnityEngine;

public class DeployableAnimationController : MonoBehaviour
{
	[SerializeField]
	private string _inAnimation;

	[SerializeField]
	private string _loopAnimation;

	[SerializeField]
	private string _outAnimation;

	[SerializeField]
	private float _totalDuration;

	[SerializeField]
	private string _itemName = string.Empty;

	private AnimationState inAnim;

	private AnimationState loopAnim;

	private AnimationState outAnim;

	private void Start()
	{
		Item itemByName = ServiceManager.Instance.GetItemByName(_itemName);
		if (itemByName != null)
		{
			itemByName.UpdateProperty("timeToLive", ref _totalDuration, "|");
		}
		inAnim = base.GetComponent<Animation>()[_inAnimation];
		if (!string.IsNullOrEmpty(_loopAnimation))
		{
			loopAnim = base.GetComponent<Animation>()[_loopAnimation];
		}
		outAnim = base.GetComponent<Animation>()[_outAnimation];
		StartCoroutine(DoAnimations());
	}

	private IEnumerator DoAnimations()
	{
		float timeRemaining2 = _totalDuration;
		base.GetComponent<Animation>().Play(inAnim.name);
		timeRemaining2 -= inAnim.length;
		yield return new WaitForSeconds(inAnim.length);
		if (loopAnim != null)
		{
			loopAnim.wrapMode = WrapMode.Loop;
			base.GetComponent<Animation>().Play(loopAnim.name);
		}
		yield return new WaitForSeconds(timeRemaining2 - outAnim.length);
		base.GetComponent<Animation>().Play(outAnim.name);
		yield return new WaitForSeconds(outAnim.length);
	}
}
