using UnityEngine;

public class ResourcesMusicLoader : MonoBehaviour
{
	public string musicName;

	private void Start()
	{
		SoundManager.Instance.playMusic(Resources.Load("Sounds/Music/" + musicName) as AudioClip, true);
	}
}
