using UnityEngine;

public class TextProcessor : FontChooser
{
	public bool WordWrap;

	public bool DestroyColliderAfterInitilization = true;

	protected Renderer textRenderer;

	private Vector2 textBoundsSize;

	private bool hasBounds;

	public Vector2 sizeOffset;

	protected override void Awake()
	{
		base.Awake();
		textRenderer = textMesh.GetComponent<Renderer>();
		if (textRenderer == null)
		{
			Debug.LogError("FontResizer " + base.name + " doesn't have a Renderer component.");
		}
		BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
		if (component != null)
		{
			textBoundsSize.x = component.size.x;
			textBoundsSize.y = component.size.y;
			hasBounds = true;
			if (DestroyColliderAfterInitilization)
			{
				Object.Destroy(component);
			}
		}
	}

	public void OnSetText(string text)
	{
		if (text == null)
		{
			textMesh.text = string.Empty;
		}
		else
		{
			OnSetTextNoLocalizing(text);
		}
	}

	public void OnSetTextNoLocalizing(string translatedText)
	{
		bool activeInHierarchy = textRenderer.gameObject.activeInHierarchy;
		textRenderer.gameObject.SetActive(true);
		textMesh.text = translatedText;
		textMesh.characterSize = originalCharacterSize;
		string[] array = ((!hasBounds || !WordWrap) ? new string[0] : translatedText.Split(' '));
		if (hasBounds)
		{
			Quaternion rotation = textRenderer.transform.rotation;
			textRenderer.transform.rotation = Quaternion.identity;
			while ((textRenderer.bounds.size.x > textBoundsSize.x + sizeOffset.x || textRenderer.bounds.size.y > textBoundsSize.y + sizeOffset.y) && textMesh.characterSize > 0f)
			{
				if (WordWrap)
				{
					textMesh.text = string.Empty;
					if (array.Length <= 0)
					{
						return;
					}
					textMesh.text = array[0];
					for (int i = 1; i < array.Length; i++)
					{
						string text = textMesh.text;
						textMesh.text = textMesh.text + " " + array[i];
						if (textRenderer.bounds.size.x > textBoundsSize.x)
						{
							textMesh.text = text + "\n" + array[i];
						}
					}
				}
				if ((textRenderer.bounds.size.x > textBoundsSize.x + sizeOffset.x || textRenderer.bounds.size.y > textBoundsSize.y + sizeOffset.y) && textMesh.characterSize > 0f)
				{
					textMesh.characterSize -= 0.1f;
				}
			}
			textRenderer.transform.rotation = rotation;
		}
		textRenderer.gameObject.SetActive(activeInHierarchy);
	}

	public bool IsNullOrEmpty()
	{
		return string.IsNullOrEmpty(textMesh.text);
	}

	public float getTextHeight()
	{
		return textRenderer.bounds.size.y;
	}
}
