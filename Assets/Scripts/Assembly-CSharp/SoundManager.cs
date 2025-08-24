using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	private float effectsVolume = 0.1f;

	private float musicVolume = 0.01f;

	private static SoundManager instance;

	public AudioClip effectChangeSound;

	private AudioSource musicAudio;

	private bool playingEffectChange;

	public AudioSource MusicAudio
	{
		get
		{
			return musicAudio;
		}
	}

	public static SoundManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(SoundManager)) as SoundManager;
				if (instance == null)
				{
					return null;
				}
			}
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		musicAudio = base.audio;
		musicAudio.ignoreListenerVolume = true;
		if (ServiceManager.Instance.GetStats() != null && ServiceManager.Instance.GetStats().pid != -1)
		{
			musicVolume = PlayerPrefs.GetFloat("musicVolume" + ServiceManager.Instance.GetStats().pid, 0.5f);
			effectsVolume = PlayerPrefs.GetFloat("effectsVolume" + ServiceManager.Instance.GetStats().pid, 0.5f);
		}
		else
		{
			musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
			effectsVolume = PlayerPrefs.GetFloat("effectsVolume", 0.5f);
		}
		if (effectsVolume < 0.05f)
		{
			effectsVolume = 0f;
		}
		AudioListener.volume = effectsVolume;
		if (musicVolume < 0.05f)
		{
			musicVolume = 0f;
		}
		musicAudio.volume = musicVolume;
	}

	public void setEffectsVolume(float v)
	{
		if (v < 0.05f)
		{
			v = 0f;
		}
		effectsVolume = v;
		AudioListener.volume = effectsVolume;
		if (!playingEffectChange)
		{
			AudioSource.PlayClipAtPoint(effectChangeSound, base.transform.position, effectsVolume);
			StartCoroutine(effectChangeCountdown());
		}
		PlayerPrefs.SetFloat("effectsVolume", effectsVolume);
		if (ServiceManager.Instance.GetStats() != null && ServiceManager.Instance.GetStats().pid != -1)
		{
			PlayerPrefs.SetFloat("effectsVolume" + ServiceManager.Instance.GetStats().pid, effectsVolume);
		}
	}

	private IEnumerator effectChangeCountdown()
	{
		playingEffectChange = true;
		yield return new WaitForSeconds(effectChangeSound.length);
		playingEffectChange = false;
	}

	public void setMusicVolume(float v)
	{
		if (v < 0.05f)
		{
			v = 0f;
		}
		musicVolume = v;
		musicAudio.volume = v;
		PlayerPrefs.SetFloat("musicVolume", musicVolume);
		if (ServiceManager.Instance.GetStats() != null && ServiceManager.Instance.GetStats().pid != -1)
		{
			PlayerPrefs.SetFloat("musicVolume" + ServiceManager.Instance.GetStats().pid, musicVolume);
		}
	}

	public float getEffectsVolume()
	{
		return effectsVolume;
	}

	public float getMusicVolume()
	{
		return musicVolume;
	}

	public void pauseMusic()
	{
		musicAudio.Pause();
	}

	public void resumeMusic()
	{
		musicAudio.Play();
	}

	public void playMusic(AudioClip c, bool loopMode)
	{
		if (musicAudio != null)
		{
			musicAudio.Stop();
		}
		musicAudio.loop = loopMode;
		musicAudio.clip = c;
		musicAudio.Play();
	}

	public void stopAll()
	{
		musicAudio.Stop();
		musicAudio.clip = null;
	}
}
