using System;
using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtensions
{
	public static Coroutine StartCoroutine(this MonoBehaviour mono, IEnumerator enumerator, Action callback)
	{
		return mono.StartCoroutine(RunCoroutineWithCallback(enumerator, callback));
	}

	private static IEnumerator RunCoroutineWithCallback(IEnumerator enumerator, Action callback)
	{
		yield return enumerator;
		if (callback != null)
		{
			callback();
		}
	}
}
