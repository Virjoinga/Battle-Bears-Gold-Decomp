using UnityEngine;

public class GUICooldownButton : GUIRoundButton
{
	private bool _onCooldown;

	private float _totalCooldownTime;

	private float _nextUseTime;

	private float _lastUseTime;

	public Texture2D FullCooldownTexture { get; set; }

	public Texture2D HalfCooldownTexture { get; set; }

	public Texture2D HalfButtonTexture { get; set; }

	public void StartCooldown(float nextUseTime, float lastUseTime)
	{
		_onCooldown = true;
		if (lastUseTime < 0f)
		{
			_lastUseTime = Time.time;
		}
		else
		{
			_lastUseTime = lastUseTime;
		}
		_nextUseTime = nextUseTime;
		_totalCooldownTime = _nextUseTime - _lastUseTime;
	}

	public void StartCooldown(float nextUseTime)
	{
		StartCooldown(nextUseTime, -1f);
	}

	public override void RenderGUI()
	{
		if (!_onCooldown)
		{
			base.RenderGUI();
			return;
		}
		float num = Time.time - _lastUseTime;
		DrawBaseButton();
		Rect position = RightSideDrawRect();
		if (num < _totalCooldownTime / 2f)
		{
			GUI.color = Preferences.Instance.HUDUnPressedButtonColor;
			GUI.DrawTexture(_backgroundDrawRect, FullCooldownTexture);
			GUI.color = Preferences.Instance.HUDPressedButtonColor;
			Matrix4x4 matrix = GUI.matrix;
			GUIUtility.RotateAroundPivot(360f * (num / _totalCooldownTime), new Vector2(base.ButtonX, base.ButtonY));
			GUI.DrawTexture(position, HalfCooldownTexture, ScaleMode.ScaleToFit, true);
			GUI.matrix = matrix;
			matrix = GUI.matrix;
			GUIUtility.RotateAroundPivot(180f, new Vector2(base.ButtonX, base.ButtonY));
			GUI.DrawTexture(position, HalfCooldownTexture, ScaleMode.ScaleToFit, true);
			GUI.matrix = matrix;
		}
		else if (num < _totalCooldownTime)
		{
			GUI.color = Preferences.Instance.HUDUnPressedButtonColor;
			GUI.DrawTexture(_backgroundDrawRect, FullCooldownTexture);
			GUI.color = Preferences.Instance.HUDPressedButtonColor;
			Matrix4x4 matrix2 = GUI.matrix;
			GUIUtility.RotateAroundPivot(360f * (num / _totalCooldownTime), new Vector2(base.ButtonX, base.ButtonY));
			GUI.DrawTexture(position, HalfCooldownTexture, ScaleMode.ScaleToFit, true);
			GUI.matrix = matrix2;
			GUI.color = Preferences.Instance.HUDUnPressedButtonColor;
			GUI.DrawTexture(position, HalfCooldownTexture, ScaleMode.ScaleToFit, true);
		}
		else
		{
			_onCooldown = false;
		}
	}

	private void DrawBaseButton()
	{
		if (base.BackgroundTexture != null)
		{
			Color color = (GUI.color = Preferences.Instance.HUDPressedButtonColor);
			GUI.color = color;
			GUI.DrawTexture(_backgroundDrawRect, base.BackgroundTexture);
		}
		if (base.Icon != null)
		{
			GUI.color = Preferences.Instance.HUDButtonIconColor;
			GUI.DrawTexture(_iconDrawRect, base.Icon);
		}
	}

	private Rect RightSideDrawRect()
	{
		return new Rect(_backgroundDrawRect.xMin + _backgroundDrawRect.width / 2f, _backgroundDrawRect.yMin, _backgroundDrawRect.width / 2f, _backgroundDrawRect.height);
	}

	private Rect LeftSideDrawRect()
	{
		return new Rect(_backgroundDrawRect.xMin, _backgroundDrawRect.yMin, _backgroundDrawRect.width / 2f, _backgroundDrawRect.height);
	}
}
