using UnityEngine;

public class SetPlayerPrefOnClosePopup : TutorialPopup
{
	[SerializeField]
	private string playerPrefToSet;

	[SerializeField]
	private int integerValueToSet;

	protected override void OnClose()
	{
		base.OnClose();
		if (!string.IsNullOrEmpty(playerPrefToSet))
		{
			PlayerPrefs.SetInt(playerPrefToSet, integerValueToSet);
		}
	}
}
