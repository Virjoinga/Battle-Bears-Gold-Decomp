using UnityEngine;

public class EffectChooser : MonoBehaviour
{
	private void Awake()
	{
		if (BBRQuality.HighRes)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Object.Destroy(this);
		}
	}
}
