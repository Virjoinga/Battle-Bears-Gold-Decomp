using UnityEngine;

public class LevelQualitySettings : MonoBehaviour
{
	private static LevelQualitySettings _instance;

	[SerializeField]
	private GraphicsLevel[] _graphicsLevels = new GraphicsLevel[0];

	public static LevelQualitySettings Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	public void ModifyCharacter(PlayerController character)
	{
		if (!(character != null))
		{
			return;
		}
		GraphicsLevel[] graphicsLevels = _graphicsLevels;
		foreach (GraphicsLevel graphicsLevel in graphicsLevels)
		{
			if (graphicsLevel._qualityLevel == BBRQuality.Current)
			{
				graphicsLevel.ModifyCharacter(character);
				break;
			}
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}
}
