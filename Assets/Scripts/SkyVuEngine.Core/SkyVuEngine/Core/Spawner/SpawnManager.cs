using System;
using System.Collections.Generic;

namespace SkyVuEngine.Core.Spawner
{
	public class SpawnManager
	{
		public enum SpawningStrategyType
		{
			Area = 0,
			Wave = 1
		}

		private static SpawnManager _instance = null;

		private Dictionary<int, ISpawningStrategy> _spawnStrategyDictionary = new Dictionary<int, ISpawningStrategy>();

		public static SpawnManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new SpawnManager();
				}
				return _instance;
			}
		}

		private SpawnManager()
		{
		}

		public ISpawningStrategy AddSpawningStrategy(int ID, SpawningStrategyType strategyType, List<ISpawnPoint> spawnPoints)
		{
			if (!_spawnStrategyDictionary.ContainsKey(ID))
			{
				switch (strategyType)
				{
				case SpawningStrategyType.Area:
					_spawnStrategyDictionary.Add(ID, new SpawningArea(spawnPoints));
					break;
				case SpawningStrategyType.Wave:
					_spawnStrategyDictionary.Add(ID, new SpawningWave(spawnPoints));
					break;
				default:
					throw new ArgumentException("The spawning strategy type: " + strategyType.ToString() + " does not exist.");
				}
				_spawnStrategyDictionary[ID].AssignSpawnStrategyIDToSpawnPoints(ID);
				return _spawnStrategyDictionary[ID];
			}
			throw new ArgumentException("The spawning strategy with ID: " + ID + " already exists.");
		}

		public ISpawningStrategy AddSpawningStrategy(int ID, ISpawningStrategy strategy)
		{
			if (!_spawnStrategyDictionary.ContainsKey(ID))
			{
				_spawnStrategyDictionary.Add(ID, strategy);
				_spawnStrategyDictionary[ID].AssignSpawnStrategyIDToSpawnPoints(ID);
				return _spawnStrategyDictionary[ID];
			}
			throw new ArgumentException("The spawning strategy with ID: " + ID + " already exists.");
		}

		public ISpawningStrategy GetSpawningStrategy(int ID)
		{
			if (_spawnStrategyDictionary.ContainsKey(ID))
			{
				return _spawnStrategyDictionary[ID];
			}
			throw new NullReferenceException("The spawning strategy with ID:" + ID + " was not found.");
		}

		public void RemoveSpawningStrategy(int ID)
		{
			if (_spawnStrategyDictionary.ContainsKey(ID))
			{
				_spawnStrategyDictionary.Remove(ID);
			}
		}

		public void Shutdown()
		{
			_spawnStrategyDictionary.Clear();
		}

		public bool GetSpawningStrategyIsSpawning(int ID)
		{
			if (_spawnStrategyDictionary.ContainsKey(ID))
			{
				return _spawnStrategyDictionary[ID].IsSpawning;
			}
			throw new NullReferenceException("The spawning strategy with ID:" + ID + " was not found.");
		}

		public void SetSpawningStrategyIsSpawning(int ID, bool value)
		{
			if (_spawnStrategyDictionary.ContainsKey(ID))
			{
				_spawnStrategyDictionary[ID].IsSpawning = value;
			}
		}

		public bool GetSpawningStrategyCanSpawn(int ID)
		{
			if (_spawnStrategyDictionary.ContainsKey(ID))
			{
				return _spawnStrategyDictionary[ID].CanSpawn;
			}
			throw new NullReferenceException("The spawning area with ID: " + ID + " was not found.");
		}

		public void EnemyDied(int ID, int threatValue)
		{
			if (_spawnStrategyDictionary.ContainsKey(ID))
			{
				_spawnStrategyDictionary[ID].DecreaseThreat(threatValue);
			}
		}

		public void UpdateSpawningStrategy(float time)
		{
			Dictionary<int, ISpawningStrategy>.Enumerator enumerator = _spawnStrategyDictionary.GetEnumerator();
			do
			{
				if (enumerator.Current.Value != null && enumerator.Current.Value.IsSpawning)
				{
					enumerator.Current.Value.Update(time);
				}
			}
			while (enumerator.MoveNext());
		}

		public void RemoveAllPlayers()
		{
			Dictionary<int, ISpawningStrategy>.Enumerator enumerator = _spawnStrategyDictionary.GetEnumerator();
			do
			{
				if (enumerator.Current.Value != null)
				{
					enumerator.Current.Value.RemoveAllPlayers();
				}
			}
			while (enumerator.MoveNext());
		}
	}
}
