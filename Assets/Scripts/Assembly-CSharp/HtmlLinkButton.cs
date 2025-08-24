using UnityEngine;

public class HtmlLinkButton : MonoBehaviour
{
	public string HtmlLink;

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (b.name == base.name)
		{
			Application.OpenURL(HtmlLink);
		}
	}
}
