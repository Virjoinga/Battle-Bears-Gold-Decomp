using UnityEngine;

public class ModeSelect : MonoBehaviour
{
	private int _modeIndex;

	[SerializeField]
	private GameObject[] _modeImages;

	private void Awake()
	{
		for (int i = 0; i < _modeImages.Length; i++)
		{
			_modeImages[i].SetActive(false);
		}
	}

	public string ChangeMode(int amount)
	{
		_modeImages[_modeIndex].SetActive(false);
		_modeIndex += amount;
		if (_modeIndex < 0)
		{
			_modeIndex += _modeImages.Length;
		}
		else if (_modeIndex >= _modeImages.Length)
		{
			_modeIndex = 0 + (_modeIndex - _modeImages.Length);
		}
		_modeImages[_modeIndex].SetActive(true);
		return ((GameMode)_modeIndex).ToString();
	}

	public void SetMode(GameMode mode)
	{
		_modeImages[_modeIndex].SetActive(false);
		_modeIndex = (int)mode;
		_modeImages[_modeIndex].SetActive(true);
	}
}
