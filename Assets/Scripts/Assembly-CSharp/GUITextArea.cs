using UnityEngine;

public class GUITextArea : GUIComponent, RenderedGUIComponent
{
	private static int DEFAULT_FONT_SIZE = 42;

	private Rect _textDrawRect;

	public bool Enabled { get; set; }

	public float PositionX { get; set; }

	public float PositionY { get; set; }

	public float DefaultWidth { get; set; }

	public float DefaultHeight { get; set; }

	public float Width { get; set; }

	public float Height { get; set; }

	public bool CenterOnPosXY { get; set; }

	public bool UseWhiteTint { get; set; }

	public bool UseBlackTint { get; set; }

	public float RenderDepth { get; set; }

	public string Text { get; set; }

	public int FontSize { get; set; }

	public void AddTo(PlayerGUI gui)
	{
		gui.AddRenderedComponent(RenderDepth, this);
		if (FontSize == 0)
		{
			FontSize = DEFAULT_FONT_SIZE;
		}
		Enabled = true;
	}

	public void RemoveFrom(PlayerGUI gui)
	{
		gui.RemoveRenderedComponent(this);
	}

	public void RenderGUI()
	{
		if (Enabled && Text != null)
		{
			if (CenterOnPosXY)
			{
				_textDrawRect = new Rect(PositionX - Width / 2f, PositionY - Height / 2f, Width, Height);
			}
			else
			{
				_textDrawRect = new Rect(PositionX, PositionY, Width, Height);
			}
			if (UseWhiteTint)
			{
				GUI.color = Color.white;
			}
			else if (UseBlackTint)
			{
				GUI.color = Color.black;
			}
			else
			{
				GUI.color = Preferences.Instance.HUDColor;
			}
			GUIStyle style = GUI.skin.GetStyle("Label");
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = (int)((float)FontSize * PlayerGUI.Instance.SmallestRatio);
			style.fontStyle = FontStyle.Bold;
			GUI.Label(_textDrawRect, Text);
		}
	}
}
