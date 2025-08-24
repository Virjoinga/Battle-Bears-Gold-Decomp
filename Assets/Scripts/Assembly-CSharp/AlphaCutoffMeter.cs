using System.Collections;
using UnityEngine;

public class AlphaCutoffMeter : MonoBehaviour
{
	public float lowestPercent;

	public float highestPercent = 1f;

	public void FillOverDuration(float startPercent, float endPercent, float seconds)
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(FillOverDurationCoroutine(startPercent, endPercent, seconds));
		}
	}

	public void SetPercent(float percent)
	{
		if (percent < lowestPercent)
		{
			percent = lowestPercent;
		}
		else if (percent > highestPercent)
		{
			percent = highestPercent;
		}
		if (float.IsNaN(percent))
		{
			percent = 1f;
		}
		if (base.GetComponent<Renderer>() != null && base.GetComponent<Renderer>().material != null && base.GetComponent<Renderer>().material.HasProperty("_Cutoff"))
		{
			base.GetComponent<Renderer>().material.SetFloat("_Cutoff", percent);
		}
	}

	public void StopFill()
	{
		StopAllCoroutines();
	}

	private IEnumerator FillOverDurationCoroutine(float startPercent, float endPercent, float seconds)
	{
		float currentPercent = startPercent;
		float fillRate = Mathf.Abs(startPercent - endPercent) / seconds;
		float startTime = Time.time;
		SetPercent(startPercent);
		while (Time.time < startTime + seconds)
		{
			currentPercent = Mathf.MoveTowards(currentPercent, endPercent, fillRate * Time.deltaTime);
			SetPercent(currentPercent);
			yield return null;
		}
		SetPercent(endPercent);
	}
}
