using UnityEngine;

public class RadiationMeter : SegmentedMeter
{
	public AudioClip[] radiationLevelSounds;

	public override void SetSegmentLevel(int level)
	{
		if (level < 0)
		{
			level = 0;
		}
		base.SetSegmentLevel(level);
		if (!base.enabled)
		{
			base.audio.Stop();
			return;
		}
		if (level > meterSegments.Length)
		{
			level = meterSegments.Length;
		}
		if (base.audio != null && radiationLevelSounds.Length >= level)
		{
			base.audio.Stop();
			if (level > 0)
			{
				base.audio.loop = true;
				base.audio.clip = radiationLevelSounds[level - 1];
				base.audio.Play();
			}
		}
	}
}
