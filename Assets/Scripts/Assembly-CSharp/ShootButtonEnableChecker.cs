using UnityEngine;

public class ShootButtonEnableChecker : MonoBehaviour
{
	private void OnEnable()
	{
		if (Preferences.Instance.CurrentShootMode != ShootMode.shootButton)
		{
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				transform.gameObject.SetActive(false);
			}
		}
	}
}
