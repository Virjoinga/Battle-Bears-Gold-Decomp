namespace SkyVuEngine.Core.Spawner
{
	public interface ISpawnPoint
	{
		int QueuedEnemyThreat { get; }

		int Id { get; set; }

		int SpawningStrategyID { get; set; }

		void Spawn();

		void Queue();

		void RemoveAllPlayers();
	}
}
