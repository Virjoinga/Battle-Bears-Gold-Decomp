using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		Object.Destroy(this);
	}
}
