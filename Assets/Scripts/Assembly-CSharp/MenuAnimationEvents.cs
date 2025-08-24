using UnityEngine;

public class MenuAnimationEvents : MonoBehaviour
{
	public void turnOn(string objectName)
	{
		GameObject gameObject = GameObject.Find(objectName);
		if (gameObject != null)
		{
			gameObject.SetActive(true);
		}
	}

	public void turnOff(string objectName)
	{
		GameObject gameObject = GameObject.Find(objectName);
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
	}
}
