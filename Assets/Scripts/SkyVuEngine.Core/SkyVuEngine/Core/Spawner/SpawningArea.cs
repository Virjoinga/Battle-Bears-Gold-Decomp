using System;
using System.Collections.Generic;

namespace SkyVuEngine.Core.Spawner
{
	public class SpawningArea : ISpawningStrategy
	{
		private List<ISpawnPoint> _spawnPoints;

		private int _currentThreat = 0;

		private int _maxThreat = 20;

		private Random _randomClass;

		private bool _isSpawning = false;

		protected float _lastSpawnTime = 0f;

		private float _spawnRate = 1f;

		public List<ISpawnPoint> SpawnPoints
		{
			get
			{
				return _spawnPoints;
			}
		}

		public int CurrentThreat
		{
			get
			{
				return _currentThreat;
			}
		}

		public int MaxThreat
		{
			get
			{
				return _maxThreat;
			}
			set
			{
				_maxThreat = value;
			}
		}

		public float SpawnRate
		{
			get
			{
				return _spawnRate;
			}
			set
			{
				_spawnRate = value;
			}
		}

		public bool IsSpawning
		{
			get
			{
				return _isSpawning;
			}
			set
			{
				_isSpawning = value;
			}
		}

		public bool CanSpawn
		{
			get
			{
				return _maxThreat > _currentThreat;
			}
		}

		public SpawningArea(List<ISpawnPoint> spawnPoints)
		{
			_spawnPoints = spawnPoints;
			_randomClass = new Random();
			for (int i = 0; i < _spawnPoints.Count; i++)
			{
				_spawnPoints[i].Id = i;
			}
		}

		public void DecreaseThreat(int threat)
		{
			_currentThreat -= threat;
			if (_currentThreat < 0)
			{
				_currentThreat = 0;
			}
		}

		public void SpawnFromRandomPoint()
		{
			int index = _randomClass.Next(_spawnPoints.Count);
			if (_currentThreat + _spawnPoints[index].QueuedEnemyThreat <= _maxThreat)
			{
				_currentThreat += _spawnPoints[index].QueuedEnemyThreat;
				_spawnPoints[index].Spawn();
			}
		}

		public virtual void Update(float time)
		{
			if (_isSpawning && time > _lastSpawnTime + _spawnRate)
			{
				_lastSpawnTime = time;
				SpawnFromRandomPoint();
			}
		}

		public void RemoveAllPlayers()
		{
			foreach (ISpawnPoint spawnPoint in _spawnPoints)
			{
				spawnPoint.RemoveAllPlayers();
			}
		}

		public void AssignSpawnStrategyIDToSpawnPoints(int ID)
		{
			for (int i = 0; i < _spawnPoints.Count; i++)
			{
				_spawnPoints[i].SpawningStrategyID = ID;
			}
		}
	}
}
