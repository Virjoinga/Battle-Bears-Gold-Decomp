using System.Collections.Generic;

namespace SkyVuEngine.Core.Spawner
{
	public interface ISpawningStrategy
	{
		List<ISpawnPoint> SpawnPoints { get; }

		int CurrentThreat { get; }

		bool IsSpawning { get; set; }

		bool CanSpawn { get; }

		void DecreaseThreat(int threat);

		void Update(float time);

		void RemoveAllPlayers();

		void AssignSpawnStrategyIDToSpawnPoints(int ID);
	}
}
