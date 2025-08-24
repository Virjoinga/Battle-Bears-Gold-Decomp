using UnityEngine;

public class MultipleHitProjectile : Projectile
{
	[SerializeField]
	private bool _explodeOnPlayer;

	[SerializeField]
	private int _collisionsBeforeExplode = 3;

	private int _collisionCount;

	private new void Start()
	{
		if (_explodeOnPlayer || !(GameManager.Instance != null))
		{
			return;
		}
		foreach (PlayerCharacterManager playerCharacterManager in GameManager.Instance.GetPlayerCharacterManagers())
		{
			if (playerCharacterManager.PlayerController != null && playerCharacterManager.PlayerController.GetComponent<Collider>() != null)
			{
				bool flag = playerCharacterManager.PlayerController.enabled;
				playerCharacterManager.PlayerController.GetComponent<Collider>().enabled = true;
				Physics.IgnoreCollision(base.GetComponent<Collider>(), playerCharacterManager.PlayerController.GetComponent<Collider>());
				playerCharacterManager.PlayerController.GetComponent<Collider>().enabled = flag;
			}
		}
	}

	protected override void OnCollisionEnter(Collision c)
	{
		if (!_explodeOnPlayer)
		{
			PlayerController component = c.gameObject.GetComponent<PlayerController>();
			if (component != null)
			{
				return;
			}
		}
		_collisionCount++;
		Explode(c.gameObject);
		hasSpawned = false;
	}

	public override void TryDestroy()
	{
		if (_collisionCount >= _collisionsBeforeExplode)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
