public class BassCannon : ProjectileWeapon
{
	protected override void Awake()
	{
		base.Awake();
		myAudio.clip = fireSounds[0];
		myAudio.loop = true;
	}

	protected override void PlayFireSound()
	{
	}

	public override void AnimationCreateProjectile()
	{
	}

	public override void BeginConstantFireEffects()
	{
		base.BeginConstantFireEffects();
		if (myAudio != null && fireSounds.Length > 0)
		{
			myAudio.volume = SoundManager.Instance.getEffectsVolume();
			myAudio.Play();
		}
	}

	public override void EndConstantFireEffects()
	{
		base.EndConstantFireEffects();
		if (myAudio != null && fireSounds.Length > 0)
		{
			myAudio.Pause();
		}
	}

	public override void OnReload()
	{
		base.OnReload();
		myAudio.Stop();
	}
}
