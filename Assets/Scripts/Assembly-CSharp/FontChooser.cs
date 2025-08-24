using UnityEngine;

public class FontChooser : MonoBehaviour
{
	protected TextMesh textMesh;

	protected float originalCharacterSize;

	protected virtual void Awake()
	{
		textMesh = GetComponent<TextMesh>();
		originalCharacterSize = textMesh.characterSize;
		if (textMesh != null)
		{
			if (BBRQuality.HighRes)
			{
				textMesh.font = Resources.Load("Fonts/High/" + textMesh.font.name) as Font;
				textMesh.characterSize /= 2f;
				originalCharacterSize = textMesh.characterSize;
			}
			else
			{
				textMesh.font = Resources.Load("Fonts/Low/" + textMesh.font.name) as Font;
			}
		}
	}
}
