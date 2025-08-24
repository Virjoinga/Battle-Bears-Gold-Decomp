using System;
using UnityEngine;

public class MetroTextBoxBindings : MonoBehaviour
{
	public delegate void AddTextBoxDelegate(string name, float top, float left, float height, float width);

	public delegate void RemoveTextBoxDelegate(string name);

	public delegate void SetTextForTextBoxDelegate(string name, string contents);

	public delegate void RemoveAllTextBoxesDelegate();

	public AddTextBoxDelegate addTextBox;

	public RemoveTextBoxDelegate removeTextBox;

	public RemoveAllTextBoxesDelegate removeAllTextBoxes;

	public SetTextForTextBoxDelegate setTextForTextBox;

	private static MetroTextBoxBindings _instance;

	public static MetroTextBoxBindings Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("MetroTextBoxBindings");
				_instance = gameObject.AddComponent<MetroTextBoxBindings>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return _instance;
		}
	}

	public event Action<string, string> textBoxChangedEvent;

	public void AddTextBox(string name, int top, int left, int height, int width)
	{
		if (addTextBox != null)
		{
			addTextBox(name, (float)top / (float)Screen.height, (float)left / (float)Screen.width, (float)height / (float)Screen.height, (float)width / (float)Screen.width);
		}
	}

	public void RemoveTextBox(string name)
	{
		if (removeTextBox != null)
		{
			removeTextBox(name);
		}
	}

	public void RemoveAllTextBoxes()
	{
		if (removeAllTextBoxes != null)
		{
			removeAllTextBoxes();
		}
	}

	public void SetTextForTextBox(string name, string contents)
	{
		if (setTextForTextBox != null)
		{
			setTextForTextBox(name, contents);
		}
	}

	public void TextBoxChanged(string textBoxName, string contents)
	{
		if (this.textBoxChangedEvent != null)
		{
			this.textBoxChangedEvent(textBoxName, contents);
		}
	}
}
