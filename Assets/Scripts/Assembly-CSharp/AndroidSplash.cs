using UnityEngine;

public class AndroidSplash : MonoBehaviour
{
	public Texture image;

	private Rect splashPos;

	private void Awake()
	{
		splashPos = new Rect(0f, 0f, Screen.width, Screen.height);
	}

	private void OnGUI()
	{
		if (image != null)
		{
			GUI.DrawTexture(splashPos, image);
		}
	}
}
