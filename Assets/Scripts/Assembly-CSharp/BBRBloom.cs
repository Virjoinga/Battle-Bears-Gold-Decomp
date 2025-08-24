using UnityEngine;

[AddComponentMenu("BBR Image Effects/Bloom")]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class BBRBloom : MonoBehaviour
{
	public float intensity = 1f;

	public float threshhold = 0.75f;

	public float blurWidth = 2f;

	public bool extraBlurry;

	public Material bloomMaterial;

	private bool supported;

	private RenderTexture tempRtA;

	private RenderTexture tempRtB;

	public void Start()
	{
		if (bloomMaterial == null)
		{
			bloomMaterial = (Material)Resources.Load("ImageEffects/Materials/Bloom");
		}
	}

	public bool Supported()
	{
		if (supported)
		{
			return true;
		}
		supported = SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && bloomMaterial.shader.isSupported;
		return supported;
	}

	public void CreateBuffers()
	{
		if (!tempRtA)
		{
			tempRtA = new RenderTexture(Screen.width / 4, Screen.height / 4, 0);
			tempRtA.hideFlags = HideFlags.DontSave;
		}
		if (!tempRtB)
		{
			tempRtB = new RenderTexture(Screen.width / 4, Screen.height / 4, 0);
			tempRtB.hideFlags = HideFlags.DontSave;
		}
	}

	private void OnDisable()
	{
		if ((bool)tempRtA)
		{
			Object.DestroyImmediate(tempRtA);
			tempRtA = null;
		}
		if ((bool)tempRtB)
		{
			Object.DestroyImmediate(tempRtB);
			tempRtB = null;
		}
	}

	private void OnDestroy()
	{
		if ((bool)tempRtA)
		{
			Object.DestroyImmediate(tempRtA);
			tempRtA = null;
		}
		if ((bool)tempRtB)
		{
			Object.DestroyImmediate(tempRtB);
			tempRtB = null;
		}
		if ((bool)bloomMaterial)
		{
			Object.DestroyImmediate(bloomMaterial);
			bloomMaterial = null;
		}
	}

	public bool EarlyOutIfNotSupported(RenderTexture source, RenderTexture destination)
	{
		if (!Supported())
		{
			base.enabled = false;
			Graphics.Blit(source, destination);
			return true;
		}
		return false;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateBuffers();
		if (!EarlyOutIfNotSupported(source, destination))
		{
			bloomMaterial.SetVector("_Parameter", new Vector4(0f, 0f, threshhold, intensity / (1f - threshhold)));
			float num = 1f / ((float)source.width * 1f);
			float num2 = 1f / ((float)source.height * 1f);
			bloomMaterial.SetVector("_OffsetsA", new Vector4(1.5f * num, 1.5f * num2, -1.5f * num, 1.5f * num2));
			bloomMaterial.SetVector("_OffsetsB", new Vector4(-1.5f * num, -1.5f * num2, 1.5f * num, -1.5f * num2));
			Graphics.Blit(source, tempRtB, bloomMaterial, 1);
			num *= 4f * blurWidth;
			num2 *= 4f * blurWidth;
			bloomMaterial.SetVector("_OffsetsA", new Vector4(1.5f * num, 0f, -1.5f * num, 0f));
			bloomMaterial.SetVector("_OffsetsB", new Vector4(0.5f * num, 0f, -0.5f * num, 0f));
			Graphics.Blit(tempRtB, tempRtA, bloomMaterial, 2);
			bloomMaterial.SetVector("_OffsetsA", new Vector4(0f, 1.5f * num2, 0f, -1.5f * num2));
			bloomMaterial.SetVector("_OffsetsB", new Vector4(0f, 0.5f * num2, 0f, -0.5f * num2));
			Graphics.Blit(tempRtA, tempRtB, bloomMaterial, 2);
			if (extraBlurry)
			{
				bloomMaterial.SetVector("_OffsetsA", new Vector4(1.5f * num, 0f, -1.5f * num, 0f));
				bloomMaterial.SetVector("_OffsetsB", new Vector4(0.5f * num, 0f, -0.5f * num, 0f));
				Graphics.Blit(tempRtB, tempRtA, bloomMaterial, 2);
				bloomMaterial.SetVector("_OffsetsA", new Vector4(0f, 1.5f * num2, 0f, -1.5f * num2));
				bloomMaterial.SetVector("_OffsetsB", new Vector4(0f, 0.5f * num2, 0f, -0.5f * num2));
				Graphics.Blit(tempRtA, tempRtB, bloomMaterial, 2);
			}
			bloomMaterial.SetTexture("_Bloom", tempRtB);
			Graphics.Blit(source, destination, bloomMaterial, 0);
		}
	}
}
