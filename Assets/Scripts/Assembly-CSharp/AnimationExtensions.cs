using System;
using System.Collections;
using UnityEngine;

public static class AnimationExtensions
{
	public static IEnumerator playWithCallbackCoroutine(this Animation anim, AnimationClip animationClip, Action onComplete)
	{
		anim.CrossFade(animationClip.name, Time.deltaTime);
		yield return new WaitForSeconds(anim[animationClip.name].length / Mathf.Abs(anim[animationClip.name].speed));
		if (onComplete != null)
		{
			onComplete();
		}
		yield return null;
	}

	public static IEnumerator blendWithCallbackCoroutine(this Animation anim, AnimationClip animationClip, Action onComplete)
	{
		anim.Blend(animationClip.name);
		yield return new WaitForSeconds(anim[animationClip.name].length / anim[animationClip.name].speed);
		if (onComplete != null)
		{
			onComplete();
		}
		yield return null;
	}
}
