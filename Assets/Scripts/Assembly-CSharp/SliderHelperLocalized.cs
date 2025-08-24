using UnityEngine;

public class SliderHelperLocalized : SliderHelper
{
	[SerializeField]
	private string LocalizationKey;

	private string _formatString;

	public override string PercentageText
	{
		get
		{
			return _percentageText.text;
		}
		protected set
		{
			if (string.IsNullOrEmpty(_formatString))
			{
				UpdateLocalizationText();
			}
			_percentageText.text = string.Format(_formatString, value);
		}
	}

	private void UpdateLocalizationText()
	{
		_formatString = Language.Get(LocalizationKey);
	}

	public override void SetPercentText(TextMesh textMesh, float percentage)
	{
		if (string.IsNullOrEmpty(_formatString))
		{
			UpdateLocalizationText();
		}
		percentage *= 100f;
		if (percentage < 0f)
		{
			percentage = 0f;
		}
		if (percentage > 100f)
		{
			percentage = 100f;
		}
		textMesh.text = string.Format(_formatString, (int)percentage + "%");
	}
}
