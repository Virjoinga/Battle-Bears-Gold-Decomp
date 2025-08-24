using System;
using UnityEngine;

[Serializable]
public class InteractionAnimationBundle
{
	public Animation objectToAnimate;

	public AnimationClip startToToggled;

	public AnimationClip toggledToStart;

	public AudioSource audioSource;

	public AudioClip soundToPlayOnActivation;

	public AudioClip startToToggledSound;

	public AudioClip toggledToStartSound;
}
