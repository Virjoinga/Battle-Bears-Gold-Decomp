using System;
using UnityEngine;

public static class BBRQuality
{
	private static QualitySetting _currentSetting = QualitySetting.LOWEST;

	private static string[] _settingNames = new string[5] { "Lowest", "Low", "Med", "High", "Ultra" };

	private static string[] _textureQuality = new string[5] { "Low", "High", "High", "High", "High" };

	private static string[] _skinQuality = new string[5] { "Lowest", "Low", "Med", "High", "High" };

	private static string[] _prefabQuality = new string[5] { "_Low", "_Low", "_Med", "_Med", "_High" };

	public static QualitySetting Current
	{
		get
		{
			return _currentSetting;
		}
		set
		{
			_currentSetting = value;
			Time.fixedDeltaTime = 0.04f;
			Debug.Log("Selected Quality: " + QualityName);
		}
	}

	public static string QualityName
	{
		get
		{
			return _settingNames[(int)_currentSetting];
		}
	}

	public static string TextureQuality
	{
		get
		{
			return _textureQuality[(int)_currentSetting];
		}
	}

	public static string SkinQuality
	{
		get
		{
			return _skinQuality[(int)_currentSetting];
		}
	}

	public static string PrefabQuality
	{
		get
		{
			return _prefabQuality[(int)_currentSetting];
		}
	}

	public static bool HighRes
	{
		get
		{
			return _currentSetting > QualitySetting.LOWEST;
		}
	}

	public static int SettingsCount
	{
		get
		{
			return Enum.GetValues(typeof(QualitySetting)).Length;
		}
	}

	public static void AutoDetectQuality()
	{
		Current = QualitySetting.HIGH;
	}
}
