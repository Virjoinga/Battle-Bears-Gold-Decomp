using UnityEngine;

public class SegmentedMeter : MonoBehaviour
{
	public GameObject[] meterSegments;

	public GameObject[] meterSegmentBacking;

	public GameObject[] alwaysShowOnVisible;

	public bool enabledSegmentsReplaceBacking;

	public bool Visible
	{
		get
		{
			return base.enabled;
		}
		set
		{
			if (value && !base.enabled)
			{
				GameObject[] array = meterSegmentBacking;
				foreach (GameObject gameObject in array)
				{
					gameObject.SetActive(true);
				}
				GameObject[] array2 = alwaysShowOnVisible;
				foreach (GameObject gameObject2 in array2)
				{
					gameObject2.SetActive(true);
				}
			}
			else if (!value && base.enabled)
			{
				GameObject[] array3 = meterSegmentBacking;
				foreach (GameObject gameObject3 in array3)
				{
					gameObject3.SetActive(false);
				}
				GameObject[] array4 = meterSegments;
				foreach (GameObject gameObject4 in array4)
				{
					gameObject4.SetActive(false);
				}
				GameObject[] array5 = alwaysShowOnVisible;
				foreach (GameObject gameObject5 in array5)
				{
					gameObject5.SetActive(false);
				}
			}
			base.enabled = value;
		}
	}

	public virtual void SetSegmentLevel(int level)
	{
		if (!base.enabled)
		{
			return;
		}
		if (level > meterSegments.Length)
		{
			level = meterSegments.Length;
		}
		for (int i = 0; i < meterSegments.Length; i++)
		{
			meterSegments[i].SetActive(i < level);
			if (enabledSegmentsReplaceBacking)
			{
				meterSegmentBacking[i].SetActive(i >= level);
			}
		}
	}
}
