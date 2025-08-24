using UnityEngine;

public class StatisticMod
{
	private Statistic _affectedStatistic;

	private float _duration;

	private float _expireTime;

	private float _additiveAmount;

	private float _multAmount;

	public Statistic AffectedStatistic
	{
		get
		{
			return _affectedStatistic;
		}
	}

	public float Duration
	{
		get
		{
			return _duration;
		}
	}

	public float ExpireTime
	{
		get
		{
			return _expireTime;
		}
	}

	public float AdditiveAmount
	{
		get
		{
			return _additiveAmount;
		}
	}

	public float MultAmount
	{
		get
		{
			return _multAmount;
		}
	}

	public StatisticMod(Statistic affectedStatistic, float duration, float additiveAmount, float multAmount = 1f)
	{
		_affectedStatistic = affectedStatistic;
		_duration = duration;
		_additiveAmount = additiveAmount;
		_multAmount = multAmount;
		_expireTime = Time.fixedTime + duration;
	}
}
