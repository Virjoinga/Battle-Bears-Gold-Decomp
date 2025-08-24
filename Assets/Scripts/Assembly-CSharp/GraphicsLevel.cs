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
        if (character.mainCamera == null || string.IsNullOrEmpty(_cameraEffect))
        {
            return;
        }

        Debug.Log("Adding camera effect: " + _cameraEffect);


        System.Type effectType = System.Type.GetType(_cameraEffect);
        if (effectType == null)
        {

            effectType = System.Type.GetType(_cameraEffect + ", Assembly-CSharp");
        }

        if (effectType == null)
        {
            Debug.LogError("Could not resolve camera effect type: " + _cameraEffect);
            return;
        }

        Component component = character.mainCamera.gameObject.AddComponent(effectType);


        BBRImageEffect imageEffect = component as BBRImageEffect;
        if (imageEffect != null)
        {
            foreach (EffectSetting effectSetting in _effectSettings)
            {
                Debug.Log("\tApplying setting [" + effectSetting._ID + "]: " + effectSetting._value);
                imageEffect.ApplySetting(effectSetting._ID, effectSetting._value);
            }
        }
    }

}
