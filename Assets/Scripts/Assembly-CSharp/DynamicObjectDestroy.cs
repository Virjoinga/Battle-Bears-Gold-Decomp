using UnityEngine;

public class DynamicObjectDestroy : MonoBehaviour
{
	public bool destroyIfIpadOneOr4GOnly;

	public bool destroyIfIpad;

	public bool destroyIfHighRes;

	public bool destroyIfLowRes;

	private void Awake()
	{
		if (BBRQuality.HighRes && destroyIfHighRes)
		{
			Object.Destroy(base.gameObject);
		}
		else if (Bootloader.Instance.isIpad && destroyIfIpad)
		{
			Object.Destroy(base.gameObject);
		}
		else if (destroyIfIpadOneOr4GOnly && (Bootloader.Instance.isIpadOne || Bootloader.Instance.is4G))
		{
			Object.Destroy(base.gameObject);
		}
		else if (!BBRQuality.HighRes && destroyIfLowRes)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
