using UnityEngine;

public class DisableForGameMode : MonoBehaviour
{
	[SerializeField]
	private GameMode _gameMode;

	[SerializeField]
	private GameObject[] _objectsToDisable;

	private void Awake()
	{
		if (Preferences.Instance.CurrentGameMode == _gameMode)
		{
			GameObject[] objectsToDisable = _objectsToDisable;
			foreach (GameObject gameObject in objectsToDisable)
			{
				gameObject.SetActive(false);
			}
		}
	}
}
