using System.Collections;
using UnityEngine;

public class DownloadObb : MonoBehaviour
{
	private string _mainPath;

	private string _expPath;

	private void Start()
	{
		if (!GooglePlayDownloader.RunningOnAndroid())
		{
			Application.LoadLevel("Bootloader");
			return;
		}
		_expPath = GooglePlayDownloader.GetExpansionFilePath();
		if (_expPath == null)
		{
			Debug.Log("External storage is not available!");
			return;
		}
		_mainPath = GooglePlayDownloader.GetMainOBBPath(_expPath);
		if (_mainPath == null)
		{
			GooglePlayDownloader.FetchOBB();
		}
		StartCoroutine(CoroutineLoadLevel());
	}

	protected IEnumerator CoroutineLoadLevel()
	{
		while (string.IsNullOrEmpty(_mainPath))
		{
			_mainPath = GooglePlayDownloader.GetMainOBBPath(_expPath);
			yield return new WaitForSeconds(2f);
		}
		Debug.Log("Main Path is: " + _mainPath);
		WWW loading = WWW.LoadFromCacheOrDownload("file://" + _mainPath, 0);
		yield return loading;
		if (loading.error != null)
		{
			Debug.LogError("WWW Error: " + loading.error);
		}
		else
		{
			Application.LoadLevel("Bootloader");
		}
	}
}
