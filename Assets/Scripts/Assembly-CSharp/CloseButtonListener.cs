using System.Collections;
using UnityEngine;

public class CloseButtonListener : MonoBehaviour
{
	private void OnGUIButtonClicked(GUIButton button)
	{
		switch (button.name)
		{
		case "error_close":
			DelayedSetActive(false, base.animation["out"].length);
			base.animation.Play("out");
			break;
		}
	}

	private IEnumerator DelayedSetActive(bool active, float delay)
	{
		yield return new WaitForSeconds(delay);
		base.gameObject.SetActive(false);
	}
}
