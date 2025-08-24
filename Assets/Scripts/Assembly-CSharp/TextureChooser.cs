using System;
using System.Collections.Generic;
using UnityEngine;

public class TextureChooser : MonoBehaviour
{
	private static TextureChooser instance;

	private Dictionary<string, Material> textures = new Dictionary<string, Material>();

	private int _texturesLoading;

	public static TextureChooser Instance
	{
		get
		{
			return instance;
		}
	}

	public bool IsLoading { get; set; }

	private void Awake()
	{
		instance = this;
	}

	public void OnLevelWasLoaded()
	{
		IsLoading = true;
		loadMaterialsForPath("Materials/All");
		loadMaterialsForPath("Materials/" + Application.loadedLevelName);
		GC.Collect();
		Component[] array = UnityEngine.Object.FindObjectsOfType(typeof(Camera)) as Component[];
		Component[] array2 = array;
		foreach (Component component in array2)
		{
			(component as Camera).enabled = true;
		}
	}

	private void loadMaterialsForPath(string path)
	{
		UnityEngine.Object[] array = Resources.LoadAll(path, typeof(Material));
		for (int i = 0; i < array.Length; i++)
		{
			Material material = array[i] as Material;
			string[] array2 = material.name.Split('_');
			if (textures.ContainsKey(array2[0]))
			{
				material.mainTexture = textures[array2[0]].mainTexture;
				continue;
			}
			if (BBRQuality.HighRes)
			{
				if (array2[0].Contains("HELVETICA"))
				{
					Font font = Resources.Load("Fonts/High/HELVETICA") as Font;
					if (font != null)
					{
						material.mainTexture = font.material.mainTexture;
					}
				}
				else
				{
					material.mainTexture = Resources.Load("Textures/High/" + array2[0]) as Texture2D;
				}
			}
			else if (array2[0].Contains("HELVETICA"))
			{
				Font font2 = Resources.Load("Fonts/Low/HELVETICA") as Font;
				if (font2 != null)
				{
					material.mainTexture = font2.material.mainTexture;
				}
			}
			else
			{
				material.mainTexture = Resources.Load("Textures/Low/" + array2[0]) as Texture2D;
			}
			textures.Add(array2[0], material);
		}
		textures.Clear();
	}

	public Texture2D LoadTexture(string name)
	{
		Texture2D texture2D = null;
		if (BBRQuality.Current > QualitySetting.LOWEST)
		{
			return Resources.Load("Textures/High/" + name) as Texture2D;
		}
		return Resources.Load("Textures/Low/" + name) as Texture2D;
	}
}
