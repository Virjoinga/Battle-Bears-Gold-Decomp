using UnityEngine;

public class DestroyIfNotDebug : MonoBehaviour
{
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
