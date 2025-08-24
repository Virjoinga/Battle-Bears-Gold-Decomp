using UnityEngine;

public class RadarPauseToggleDisabler : MonoBehaviour
{
	private void Start()
	{
		if (!HUD.Instance.RadarPurchased)
		{
			base.gameObject.SetActive(false);
		}
	}
}
