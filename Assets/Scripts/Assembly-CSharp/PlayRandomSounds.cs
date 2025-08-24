using System.Collections;
using UnityEngine;

public class PlayRandomSounds : MonoBehaviour
{
	private AudioSource myAudio;

	public AudioClip[] clips;

	public float minTimeBetweenSounds;

	public float maxTimeBetweenSounds;

	private void Awake()
	{
		myAudio = base.audio;
	}

	private void Start()
	{
		StartCoroutine(playRandomSounds());
	}

	private IEnumerator playRandomSounds()
	{
		while (myAudio != null)
		{
			int soundIndex = Random.Range(0, clips.Length);
			if (clips[soundIndex] != null)
			{
				myAudio.PlayOneShot(clips[soundIndex]);
			}
			yield return new WaitForSeconds(Mathf.Max(0.1f, Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds)));
		}
	}
}
