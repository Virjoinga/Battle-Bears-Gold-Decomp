using UnityEngine;

public class MultiObjectToggler : MonoBehaviour
{
	public GameObject[] texts;

	private void OnEnable()
	{
		GameObject[] array = texts;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				gameObject.SetActiveRecursively(true);
			}
		}
	}

	private void OnDisable()
	{
		GameObject[] array = texts;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				gameObject.SetActiveRecursively(false);
			}
		}
	}
}
