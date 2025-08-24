using System.Collections;
using UnityEngine;

public class MemorySweep : MonoBehaviour
{
	public static string levelToLoad;

	private void Awake()
	{
		Resources.UnloadUnusedAssets();
		StartCoroutine(delayedLoadLevel());
	}

	protected IEnumerator delayedLoadLevel()
	{
		yield return Application.LoadLevelAsync(levelToLoad);
	}
}
