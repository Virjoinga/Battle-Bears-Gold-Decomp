using UnityEngine;

public class ErrorMessage : Popup
{
	public TextMesh title;

	public TextMesh body;

	public TextMesh buttonText;

	private string levelName = string.Empty;

	public AudioClip[] clickSounds;

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (clickSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(clickSounds[Random.Range(0, clickSounds.Length)], Vector3.zero);
		}
		switch (b.name)
		{
		case "backBtn":
			if (levelName == string.Empty)
			{
				OnClose();
			}
			else
			{
				Application.LoadLevel(levelName);
			}
			break;
		}
	}

	public void setErrorText(string t, string b, string buttonTitle, string level)
	{
		title.text = t;
		body.text = b;
		buttonText.text = buttonTitle;
		levelName = level;
	}
}
