using System.Collections.Generic;
using UnityEngine;

public class StatisticManager
{
	private Dictionary<Statistic, float> _baseStats = new Dictionary<Statistic, float>();

	private Dictionary<Statistic, float> _currentStats = new Dictionary<Statistic, float>();

	private List<StatisticMod> _statMods = new List<StatisticMod>();

	private PlayerController _owner;

	public float this[Statistic stat]
	{
		get
		{
			return _currentStats[stat];
		}
	}

	public StatisticManager(PlayerController controller)
	{
		_owner = controller;
		SetupBaseStats(controller);
	}

	private void SetupBaseStats(PlayerController controller)
	{
		_baseStats.Add(Statistic.MaxBackwardsMovementSpeed, controller.Motor.movement.maxBackwardsSpeed);
		_baseStats.Add(Statistic.MaxForwardMovementSpeed, controller.Motor.movement.maxForwardSpeed);
		_baseStats.Add(Statistic.MaxSidewaysMovementSpeed, controller.Motor.movement.maxSidewaysSpeed);
		_baseStats.Add(Statistic.SpeedModAdditive, 0f);
		_baseStats.Add(Statistic.SpeedModMult, 1f);
		ResetCurrentStats();
	}

	private void RecalculateStats()
	{
		ResetCurrentStats();
		foreach (StatisticMod statMod in _statMods)
		{
			AddModToCurrentStats(statMod);
		}
		ApplyOutsideStatistics();
	}

	private void ResetCurrentStats()
	{
		_currentStats.Clear();
		_currentStats.Merge(_baseStats);
	}

	private void AddModToCurrentStats(StatisticMod mod)
	{
		if (mod.AdditiveAmount != 0f)
		{
			Dictionary<Statistic, float> currentStats;
			Dictionary<Statistic, float> dictionary = (currentStats = _currentStats);
			Statistic affectedStatistic;
			Statistic key = (affectedStatistic = mod.AffectedStatistic);
			float num = currentStats[affectedStatistic];
			dictionary[key] = num + mod.AdditiveAmount;
		}
		if (mod.MultAmount != 1f)
		{
			Dictionary<Statistic, float> currentStats2;
			Dictionary<Statistic, float> dictionary2 = (currentStats2 = _currentStats);
			Statistic affectedStatistic;
			Statistic key2 = (affectedStatistic = mod.AffectedStatistic);
			float num = currentStats2[affectedStatistic];
			dictionary2[key2] = num * mod.MultAmount;
		}
	}

	private void ApplyOutsideStatistics()
	{
		foreach (KeyValuePair<Statistic, float> currentStat in _currentStats)
		{
			switch (currentStat.Key)
			{
			case Statistic.MaxBackwardsMovementSpeed:
				_owner.Motor.movement.maxBackwardsSpeed = ((!(currentStat.Value < 0f)) ? currentStat.Value : 0f);
				break;
			case Statistic.MaxForwardMovementSpeed:
				_owner.Motor.movement.maxForwardSpeed = ((!(currentStat.Value < 0f)) ? currentStat.Value : 0f);
				break;
			case Statistic.MaxSidewaysMovementSpeed:
				_owner.Motor.movement.maxSidewaysSpeed = ((!(currentStat.Value < 0f)) ? currentStat.Value : 0f);
				break;
			}
		}
	}

	public void AddStatMod(StatisticMod mod)
	{
		_statMods.Add(mod);
		RecalculateStats();
	}

	public void RemoveStatMod(StatisticMod mod)
	{
		_statMods.Remove(mod);
		RecalculateStats();
	}

	public void RemoveExpiredMods()
	{
		List<StatisticMod> list = new List<StatisticMod>();
		foreach (StatisticMod statMod in _statMods)
		{
			if (Time.fixedTime >= statMod.ExpireTime)
			{
				list.Add(statMod);
			}
		}
		foreach (StatisticMod item in list)
		{
			RemoveStatMod(item);
		}
	}
}
