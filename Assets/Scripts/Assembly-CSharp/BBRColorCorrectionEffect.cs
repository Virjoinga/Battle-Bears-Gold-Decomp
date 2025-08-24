using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("BBR Image Effects/Color Correction")]
public class BBRColorCorrectionEffect : MonoBehaviour, BBRImageEffect
{
	private Material material;

	private string _correctionTexture = "ImageEffects/Textures/ContrastEnhanced";

	public Texture textureRamp;

	public void ApplySetting(string key, string val)
	{
		if (key.Equals("tex"))
		{
			_correctionTexture = val;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (material == null)
		{
			material = (Material)Resources.Load("ImageEffects/Materials/ColorCorrect");
		}
		if (textureRamp == null)
		{
			textureRamp = (Texture)Resources.Load(_correctionTexture);
		}
		material.SetTexture("_RampTex", textureRamp);
		Graphics.Blit(source, destination, material);
	}

	public void OnDestoy()
	{
		if (material != null)
		{
			Object.Destroy(material);
		}
		if (textureRamp != null)
		{
			Object.Destroy(textureRamp);
		}
	}
}
