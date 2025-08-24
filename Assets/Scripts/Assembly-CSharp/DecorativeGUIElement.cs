using UnityEngine;

public class DecorativeGUIElement : GUIComponent, RenderedGUIComponent
{
	protected Rect _iconDrawRect;

	protected Rect _textureCoordinates;

	public bool Enabled { get; set; }

	public Texture2D Icon { get; set; }

	public float PositionX { get; set; }

	public float PositionY { get; set; }

	public float Width { get; set; }

	public float Height { get; set; }

	public bool CenterOnPosXY { get; set; }

	public bool UseWhiteTint { get; set; }

	public bool UseSecondaryGUIColor { get; set; }

	public float RenderDepth { get; set; }

	public void AddTo(PlayerGUI gui)
	{
		gui.AddRenderedComponent(RenderDepth, this);
		Enabled = true;
		_textureCoordinates = new Rect(0f, 0f, 1f, 1f);
	}

	public void RemoveFrom(PlayerGUI gui)
	{
		gui.RemoveRenderedComponent(this);
	}

	public void RenderGUI()
	{
		if (Enabled && Icon != null)
		{
			_iconDrawRect = CreateDrawRect();
			if (UseWhiteTint)
			{
				GUI.color = Color.white;
			}
			else if (UseSecondaryGUIColor)
			{
				GUI.color = Preferences.Instance.HUDPressedButtonColor;
			}
			else
			{
				GUI.color = Preferences.Instance.HUDColor;
			}
			GUI.DrawTextureWithTexCoords(_iconDrawRect, Icon, _textureCoordinates);
		}
	}

	public virtual Rect CreateDrawRect()
	{
		return (!CenterOnPosXY) ? new Rect(PositionX, PositionY, Width, Height) : new Rect(PositionX - Width / 2f, PositionY - Height / 2f, Width, Height);
	}
}
