using UnityEngine;

public class PrivateMatchButtonController : MonoBehaviour
{
	[SerializeField]
	private GUIButton _privateMatchButton;

	[SerializeField]
	private GameObject _privateMatchComingSoon;

	private void Awake()
	{
		bool val = true;
		ServiceManager.Instance.UpdateProperty("enable_private_match_button", ref val);
		_privateMatchButton.enabled = val;
		_privateMatchComingSoon.SetActive(!val);
	}
}
