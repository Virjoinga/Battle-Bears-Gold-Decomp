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
			base.GetComponent<AudioSource>().Stop();
			return;
		}
		if (level > meterSegments.Length)
		{
			level = meterSegments.Length;
		}
		if (base.GetComponent<AudioSource>() != null && radiationLevelSounds.Length >= level)
		{
			base.GetComponent<AudioSource>().Stop();
			if (level > 0)
			{
				base.GetComponent<AudioSource>().loop = true;
				base.GetComponent<AudioSource>().clip = radiationLevelSounds[level - 1];
				base.GetComponent<AudioSource>().Play();
			}
		}
	}
}
