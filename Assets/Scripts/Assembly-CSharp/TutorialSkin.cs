using UnityEngine;

public class TutorialSkin : MonoBehaviour
{
	private void Awake()
	{
		SkinnedMeshRenderer component = base.gameObject.GetComponent<SkinnedMeshRenderer>();
		if (component != null)
		{
			component.sharedMaterial.mainTexture = Resources.Load("Characters/Oliver/Skins/" + BBRQuality.TextureQuality + "/brown_red") as Texture2D;
		}
	}
}
