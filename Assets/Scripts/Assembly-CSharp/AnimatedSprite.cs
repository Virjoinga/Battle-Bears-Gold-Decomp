using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite
{
	private Texture2D _sheet;

	private Vector2 _spriteSize;

	private Dictionary<string, SpriteAnimation> _animations;

	private SpriteAnimation _currentAnimation;

	public AnimatedSprite(Texture2D sheet, Vector2 spriteSize)
	{
		_animations = new Dictionary<string, SpriteAnimation>();
		_sheet = sheet;
		_spriteSize = spriteSize;
	}

	public void CreateAnimation(string name, Vector2 startingLocation, int numFrames, float timeBetweenFrames, WrapMode wrapMode, float restartPause = 0f, int xIncDir = 1, int yIncDir = 1)
	{
		SpriteAnimation value = new SpriteAnimation(name, _sheet, startingLocation, _spriteSize, numFrames, timeBetweenFrames, wrapMode, restartPause, xIncDir, yIncDir);
		_animations.Add(name, value);
	}

	public void Play(string animationName)
	{
		if (_currentAnimation != null)
		{
			_currentAnimation.Restart();
		}
		if (_animations.ContainsKey(animationName))
		{
			_currentAnimation = _animations[animationName];
		}
		else
		{
			Debug.LogWarning("A sprite animation named " + animationName + " was attempted to be played, however this sprite does not have this animation.");
		}
	}

	public void Play(string animationName, Action animationCompleteCallback)
	{
		Play(animationName);
		if (_currentAnimation != null)
		{
			_currentAnimation.AnimationCallback = animationCompleteCallback;
		}
	}

	public void Update()
	{
		if (_currentAnimation != null)
		{
			_currentAnimation.Update();
		}
	}

	public void Draw(Rect screenDrawPos)
	{
		Draw(screenDrawPos, Color.white);
	}

	public void Draw(Rect screenDrawPos, Color guiColor)
	{
		Color color = GUI.color;
		GUI.color = guiColor;
		if (_currentAnimation != null)
		{
			_currentAnimation.DrawCurrentFrame(screenDrawPos);
		}
		else
		{
			GUI.DrawTextureWithTexCoords(screenDrawPos, _sheet, new Rect(0f, 0f, _spriteSize.x / (float)_sheet.width, _spriteSize.y / (float)_sheet.height));
		}
		GUI.color = color;
	}
}
