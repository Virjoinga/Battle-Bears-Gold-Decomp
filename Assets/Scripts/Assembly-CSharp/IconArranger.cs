using UnityEngine;

public class IconArranger : MonoBehaviour
{
	public Transform[] icons;

	private Vector3 centerPos;

	private void Awake()
	{
		centerPos = Vector3.zero;
		if (icons.Length <= 0)
		{
			return;
		}
		Transform[] array = icons;
		foreach (Transform transform in array)
		{
			centerPos += transform.position;
		}
		centerPos /= (float)icons.Length;
		if (icons.Length > 0 && Bootloader.Instance.isAndroid)
		{
			icons[0].position = centerPos;
			for (int j = 1; j < icons.Length; j++)
			{
				Object.Destroy(icons[j].gameObject);
			}
		}
	}
}
