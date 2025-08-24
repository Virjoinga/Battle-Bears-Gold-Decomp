using UnityEngine;

public class TextPrompt : MonoBehaviour
{
	private TextBlock _textBlock;

	[SerializeField]
	private TextMesh _promptText;

	[SerializeField]
	private GameObject _namedObject;

	private void Awake()
	{
		if (_promptText != null)
		{
			_textBlock = _promptText.GetComponent<TextBlock>();
		}
	}

	private void Update()
	{
		if (_promptText != null)
		{
			if (_textBlock != null)
			{
				_textBlock.OnSetText(_namedObject.name, string.Empty);
			}
			else if (_namedObject != null)
			{
				_promptText.text = _namedObject.name;
			}
		}
	}
}
