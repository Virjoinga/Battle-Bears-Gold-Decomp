using UnityEngine;

public class SimpleDestroy : MonoBehaviour
{
	public void DestroyObject()
	{
		Object.Destroy(base.gameObject, 0.1f);
	}
}
