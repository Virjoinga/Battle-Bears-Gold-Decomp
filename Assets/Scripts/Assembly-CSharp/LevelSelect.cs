using System;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
	[Serializable]
	public class LevelInfo
	{
		public string name = string.Empty;

		public int x;

		public int y;
	}

	private int _levelIndex;

	[SerializeField]
	private LevelInfo[] _levels;

	[SerializeField]
	private int _levelDisplaySizeX = 4;

	[SerializeField]
	private int _levelDisplaySizeY = 4;

	[SerializeField]
	private Renderer _levelDisplay;

	private void Awake()
	{
		_levelDisplay.gameObject.SetActive(false);
	}

	public string ChangeLevel(int amount)
	{
		_levelIndex += amount;
		if (_levelIndex < 0)
		{
			_levelIndex += _levels.Length;
		}
		else if (_levelIndex >= _levels.Length)
		{
			_levelIndex = 0 + (_levelIndex - _levels.Length);
		}
		SetLevelImage(_levels[_levelIndex].x, _levels[_levelIndex].y);
		return _levels[_levelIndex].name;
	}

	public void SetLevel(string levelName)
	{
		_levelDisplay.gameObject.SetActive(true);
		for (int i = 0; i < _levels.Length; i++)
		{
			if (_levels[i].name == levelName)
			{
				_levelIndex = i;
				SetLevelImage(_levels[i].x, _levels[i].y);
				break;
			}
		}
	}

	private void SetLevelImage(int x, int y)
	{
		_levelDisplay.gameObject.SetActive(true);
		float num = 1f / (float)_levelDisplaySizeX;
		float num2 = 1f / (float)_levelDisplaySizeY;
		_levelDisplay.material.SetTextureScale("_MainTex", new Vector2(num, num2));
		_levelDisplay.material.SetTextureOffset("_MainTex", new Vector2(num * (float)x, num2 * (float)y));
	}
}
