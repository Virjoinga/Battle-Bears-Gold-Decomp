using UnityEngine;

public class PlayButtonController : MonoBehaviour
{
	[SerializeField]
	private GameObject _brTDMButton;

	[SerializeField]
	private GameObject _tdmOnlyButton;

	private void Awake()
	{
		bool val = false;
		ServiceManager.Instance.UpdateProperty("use_battle_royale_button", ref val);
		if (ServiceManager.Instance.GetStats().level < 5.0)
		{
			val = false;
		}
		_brTDMButton.SetActive(val);
		_tdmOnlyButton.SetActive(!val);
	}
}
