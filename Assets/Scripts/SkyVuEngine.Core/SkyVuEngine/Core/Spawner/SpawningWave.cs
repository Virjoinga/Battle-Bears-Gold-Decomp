using System.Collections.Generic;

namespace SkyVuEngine.Core.Spawner
{
	public class SpawningWave : ISpawningStrategy
	{
		private List<ISpawnPoint> _spawnPoints;

		private int _threatSpawned = 0;

		private int _threatAvailable = int.MaxValue;

		private int _maxThreat = int.MaxValue;

		private float _threatModifier = 1f;

		private bool _isSpawning = false;

		private float _currentTime = 0f;

		private float _lastSpawnTime = 0f;

		private float _lastWaveTime = 0f;

		private float _spawnRate = 0f;

		private float _spawnRateFloor = 0f;

		private float _spawnRateModifier = 1f;

		private float _waveRate = 0f;

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
				return _threatSpawned - _threatAvailable;
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
				_threatAvailable = (_maxThreat = value);
			}
		}

		public float ThreatModifier
		{
			get
			{
				return _threatModifier;
			}
			set
			{
				_threatModifier = value;
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

		public float SpawnRateFloor
		{
			get
			{
				return _spawnRateFloor;
			}
			set
			{
				_spawnRateFloor = value;
			}
		}

		public float SpawnRateModifier
		{
			get
			{
				return _spawnRateModifier;
			}
			set
			{
				_spawnRateModifier = value;
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

		public float WaveRate
		{
			get
			{
				return _waveRate;
			}
			set
			{
				_waveRate = value;
			}
		}

		public bool CanSpawn
		{
			get
			{
				return _threatSpawned < _maxThreat && _currentTime > _lastWaveTime + _waveRate;
			}
		}

		public SpawningWave(List<ISpawnPoint> spawnPoints)
		{
			_spawnPoints = spawnPoints;
			for (int i = 0; i < _spawnPoints.Count; i++)
			{
				_spawnPoints[i].Id = i;
			}
		}

		public void DecreaseThreat(int threat)
		{
			_threatAvailable -= threat;
			if (_threatAvailable < 0)
			{
				_threatAvailable = 0;
			}
		}

		private void SpawnEnemies()
		{
			for (int i = 0; i < _spawnPoints.Count; i++)
			{
				if (_threatSpawned + _spawnPoints[i].QueuedEnemyThreat <= _maxThreat)
				{
					_threatSpawned += _spawnPoints[i].QueuedEnemyThreat;
					_spawnPoints[i].Spawn();
				}
				else
				{
					_threatAvailable -= _maxThreat - _threatSpawned;
					_threatSpawned = _maxThreat;
				}
			}
		}

		public void Update(float time)
		{
			_currentTime = time;
			if (_isSpawning)
			{
				if (CanSpawn && _currentTime > _lastSpawnTime + _spawnRate)
				{
					_lastSpawnTime = _currentTime;
					SpawnEnemies();
				}
				else if (!CanSpawn && _threatAvailable == 0)
				{
					_lastWaveTime = _currentTime;
					_lastSpawnTime = _currentTime + _waveRate;
					_maxThreat = (int)((float)_maxThreat * _threatModifier);
					_spawnRate = (_spawnRate + _spawnRateFloor) / _spawnRateModifier;
					_threatSpawned = 0;
					_threatAvailable = _maxThreat;
				}
			}
		}

		public void RemoveAllPlayers()
		{
			for (int i = 0; i < _spawnPoints.Count; i++)
			{
				_spawnPoints[i].RemoveAllPlayers();
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
