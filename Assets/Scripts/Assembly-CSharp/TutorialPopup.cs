public class TutorialPopup : Popup
{
	public void OnGUIButtonClicked(GUIButton b)
	{
		switch (b.name)
		{
		case "closeButton":
			OnClose();
			break;
		}
	}
}
