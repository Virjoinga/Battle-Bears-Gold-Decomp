using System;
using UnityEngine;

public class SpriteAnimation
{
	private Action _callback;

	private string _name;

	private Texture2D _sheet;

	private Vector2 _spriteSize;

	private Vector2 _startingLocation;

	private int _numFrames;

	private float _timeBetweenFrames;

	private float _frameStartTime;

	private WrapMode _wrapMode;

	private float _restartPauseTime;

	private int _xIncDir;

	private int _yIncDir;

	private int _currentFrameIndex;

	private Vector2 _currentLocation;

	private Rect _frameRect;

	public Action AnimationCallback
	{
		get
		{
			return _callback;
		}
		set
		{
			_callback = value;
		}
	}

	public string Name
	{
		get
		{
			return _name;
		}
	}

	public SpriteAnimation(string name, Texture2D sheet, Vector2 startingLocation, Vector2 spriteSize, int numFrames, float timeBetweenFrames, WrapMode wrapMode, float restartPauseTime, int xIncDir = 1, int yIncDir = 1)
	{
		_callback = null;
		_name = name;
		_sheet = sheet;
		_startingLocation = startingLocation;
		_spriteSize = spriteSize;
		_numFrames = numFrames;
		_timeBetweenFrames = timeBetweenFrames;
		_frameStartTime = 0f;
		_wrapMode = wrapMode;
		_restartPauseTime = restartPauseTime;
		_xIncDir = xIncDir;
		_yIncDir = yIncDir;
		_startingLocation.y = (float)_sheet.height - (_startingLocation.y + spriteSize.y);
		_currentFrameIndex = 0;
		_currentLocation = new Vector2(_startingLocation.x, _startingLocation.y);
		_frameRect = new Rect(_currentLocation.x / (float)_sheet.width, _currentLocation.y / (float)_sheet.height, _spriteSize.x / (float)_sheet.width, _spriteSize.y / (float)_sheet.height);
	}

	public void Update()
	{
		if (Time.fixedTime - _frameStartTime > _timeBetweenFrames)
		{
			_frameStartTime = Time.fixedTime;
			MoveToNextFrame();
		}
	}

	public void DrawCurrentFrame(Rect screenDrawPos)
	{
		GUI.DrawTextureWithTexCoords(screenDrawPos, _sheet, _frameRect);
	}

	private void MoveToNextFrame()
	{
		if (_currentFrameIndex + 1 < _numFrames)
		{
			if (_xIncDir > 0)
			{
				if (_currentLocation.x + _spriteSize.x >= (float)_sheet.width)
				{
					MoveRow();
				}
				else
				{
					_currentLocation.x += _spriteSize.x;
				}
			}
			else if (_currentLocation.x - _spriteSize.x < 0f)
			{
				MoveRow();
			}
			else
			{
				_currentLocation.x -= _spriteSize.x;
			}
			_currentFrameIndex++;
			CalculateFrameRect();
		}
		else
		{
			if (_wrapMode == WrapMode.Loop)
			{
				Restart();
			}
			if (_callback != null)
			{
				_callback();
				_callback = null;
			}
		}
	}

	private void MoveRow()
	{
		_currentLocation.x = ((_xIncDir <= 0) ? ((float)_sheet.width - _spriteSize.x) : 0f);
		if (_yIncDir > 0)
		{
			_currentLocation.y -= _spriteSize.y;
		}
		else
		{
			_currentLocation.y += _spriteSize.y;
		}
	}

	public void Restart()
	{
		_currentFrameIndex = 0;
		_currentLocation.x = _startingLocation.x;
		_currentLocation.y = _startingLocation.y;
		_frameStartTime = Time.fixedTime + _restartPauseTime;
		CalculateFrameRect();
	}

	private void CalculateFrameRect()
	{
		_frameRect = new Rect(_currentLocation.x / (float)_sheet.width, _currentLocation.y / (float)_sheet.height, _spriteSize.x / (float)_sheet.width, _spriteSize.y / (float)_sheet.height);
	}
}
