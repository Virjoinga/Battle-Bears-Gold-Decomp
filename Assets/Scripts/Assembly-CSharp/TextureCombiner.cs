using UnityEngine;

public class TextureCombiner : MonoBehaviour
{
	private Renderer myRenderer;

	public Texture2D[] texturesToCombine;

	public GameObject[] models;

	public int TEXTURE_SIZE = 1024;

	public int gridX = 5;

	public int gridY = 5;

	private void Awake()
	{
		myRenderer = base.renderer;
	}

	private void Start()
	{
		Texture2D texture2D = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.RGB24, true);
		texture2D.wrapMode = TextureWrapMode.Clamp;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < texturesToCombine.Length; i++)
		{
			int width = texturesToCombine[i].width;
			int height = texturesToCombine[i].height;
			Color[] pixels = texturesToCombine[i].GetPixels();
			if (num + width > TEXTURE_SIZE)
			{
				num = 0;
				num3 = 0;
				num4++;
				num2 += height;
				if (num2 + height > TEXTURE_SIZE)
				{
					break;
				}
			}
			texture2D.SetPixels(num, num2, width, height, pixels, 0);
			Component[] componentsInChildren = models[i].GetComponentsInChildren(typeof(MeshFilter));
			float num5 = 1f / (float)gridX;
			float num6 = 1f / (float)gridY;
			Component[] array = componentsInChildren;
			foreach (Component component in array)
			{
				Mesh mesh = (component as MeshFilter).mesh;
				Vector2[] array2 = new Vector2[mesh.uv.Length];
				for (int k = 0; k < mesh.uv.Length; k++)
				{
					array2[k].x = mesh.uv[k].x / (float)gridX + (float)num3 * num5;
					array2[k].y = mesh.uv[k].y / (float)gridY + (float)num4 * num6;
				}
				mesh.uv = array2;
			}
			num += width;
			num3++;
		}
		texture2D.Apply();
		myRenderer.sharedMaterial.mainTexture = texture2D;
	}
}
