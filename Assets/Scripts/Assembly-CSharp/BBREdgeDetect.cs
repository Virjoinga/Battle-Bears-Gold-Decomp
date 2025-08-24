using UnityEngine;

[AddComponentMenu("BBR Image Effects/Edge Detect")]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class BBREdgeDetect : ImageEffectBase
{
	public float threshold = 0.2f;

	public Material edgeMaterial;

	public void OnDestoy()
	{
		if (edgeMaterial != null)
		{
			Object.Destroy(edgeMaterial);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (edgeMaterial == null)
		{
			edgeMaterial = (Material)Resources.Load("ImageEffects/Materials/Edge");
		}
		edgeMaterial.SetFloat("_Treshold", threshold * threshold);
		Graphics.Blit(source, destination, edgeMaterial);
	}
}
