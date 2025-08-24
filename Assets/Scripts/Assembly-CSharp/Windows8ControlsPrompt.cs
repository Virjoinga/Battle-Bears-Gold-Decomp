using UnityEngine;

public class Windows8ControlsPrompt : Popup
{
	public AudioClip[] clickSounds;

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (clickSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(clickSounds[Random.Range(0, clickSounds.Length)], Vector3.zero);
		}
		switch (b.name)
		{
		case "keyboard":
			Preferences.Instance.CurrentShootMode = ShootMode.keyboardAndMouse;
			OnClose();
			break;
		case "touch":
			Preferences.Instance.CurrentShootMode = ShootMode.shootButton;
			OnClose();
			break;
		}
	}
}
