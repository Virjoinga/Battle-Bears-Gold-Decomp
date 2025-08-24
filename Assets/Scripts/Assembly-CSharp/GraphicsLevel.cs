using System;
using UnityEngine;

[Serializable]
public class GraphicsLevel
{
	[SerializeField]
	public QualitySetting _qualityLevel = QualitySetting.ULTRA;

	[SerializeField]
	private string _cameraEffect = string.Empty;

	[SerializeField]
	private EffectSetting[] _effectSettings = new EffectSetting[0];

	public void ModifyCharacter(PlayerController character)
	{
		if (!(character.mainCamera != null) || _cameraEffect.Length <= 0)
		{
			return;
		}
		Debug.Log("Adding camera effect: " + _cameraEffect);
		Component component = character.mainCamera.gameObject.AddComponent(_cameraEffect);
		if (component is BBRImageEffect)
		{
			EffectSetting[] effectSettings = _effectSettings;
			foreach (EffectSetting effectSetting in effectSettings)
			{
				Debug.Log("\tApplying setting [" + effectSetting._ID + "]: " + effectSetting._value);
				((BBRImageEffect)component).ApplySetting(effectSetting._ID, effectSetting._value);
			}
		}
	}
}
