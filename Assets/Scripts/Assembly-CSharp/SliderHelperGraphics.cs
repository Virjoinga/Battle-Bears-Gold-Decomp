using UnityEngine;

public class SliderHelperGraphics : SliderHelper
{
	[SerializeField]
	private string[] _friendly_names = new string[5] { "Normal", "Good", "Gooder", "Better", "Betterer" };

	public override void SetIndicatorToWorldPos(float hitPoint)
	{
		base.SetIndicatorToWorldPos(hitPoint);
		SetIndicatorToPercent((float)PauseMenu.chosenQualitySetting / ((float)BBRQuality.SettingsCount - 1f));
	}

	public override void SetPercentText(TextMesh textMesh, float percentage)
	{
		textMesh.text = _friendly_names[(int)PauseMenu.chosenQualitySetting];
	}
}
