using System;
using UnityEngine;

[Serializable]
public class SettingsBundle
{
	public float health;

	public Animation objectToAnimate;

	public AnimationClip damagedAnimation;

	public AnimationClip destroyedAnimation;

	public AnimationClip respawnAnimation;

	public AudioSource audioSource;

	public AudioClip damagedSound;

	public AudioClip destroyedSound;

	public AudioClip respawnSound;

	public float respawnTime;
}
