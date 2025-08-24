using System;
using System.Collections;
using UnityEngine;

public class PostGameAdManager : MonoBehaviour
{
	private static PostGameAdManager _instance;

	public static PostGameAdManager Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	private void Start()
	{
		StartCoroutine(delayedMainMenu());
	}

	private IEnumerator delayedMainMenu()
	{
		yield return Resources.UnloadUnusedAssets();
		yield return new WaitForSeconds(0.25f);
		GC.Collect();
		Application.LoadLevelAsync("MainMenu");
	}
}
