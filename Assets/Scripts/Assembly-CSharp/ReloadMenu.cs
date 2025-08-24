using UnityEngine;

public class ReloadMenu : MonoBehaviour
{
	private void OnGUIButtonClicked(GUIButton button)
	{
		if (button.name == "error_close")
		{
			MainMenu.isFirstTime = true;
			BBRQuality.Current = PauseMenu.chosenQualitySetting;
			PlayerPrefs.SetInt("quality", (int)BBRQuality.Current);
			PlayerPrefs.Save();
			Application.LoadLevel("MainMenu");
			Object.Destroy(base.gameObject);
		}
	}
}
