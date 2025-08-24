using UnityEngine;

public class DestroyOnRemoveGree : MonoBehaviour
{
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
