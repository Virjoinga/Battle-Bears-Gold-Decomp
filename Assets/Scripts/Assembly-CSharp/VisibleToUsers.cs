using UnityEngine;

public class VisibleToUsers : MonoBehaviour
{
	private void OnEnable()
	{
		if (ServiceManager.Instance.GetStats().pid > 20)
		{
			base.transform.localScale = new Vector3(0f, 0f, 0f);
		}
	}
}
