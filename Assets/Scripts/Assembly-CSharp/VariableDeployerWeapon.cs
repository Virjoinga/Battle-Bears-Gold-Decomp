using UnityEngine;

public class VariableDeployerWeapon : DeployerWeapon
{
	[SerializeField]
	private int _numToDeployPerFire;

	[SerializeField]
	private float _leftAndRightSpawnOffset;

	private bool _spawnRight = true;

	public override GameObject DeployObject(int fromAnimation)
	{
		GameObject gameObject = null;
		for (int i = 0; i < _numToDeployPerFire; i++)
		{
			gameObject = base.DeployObject();
			if (gameObject != null)
			{
				if (_spawnRight)
				{
					gameObject.transform.position += base.transform.root.right.normalized * _leftAndRightSpawnOffset;
				}
				else
				{
					gameObject.transform.position += -base.transform.root.right.normalized * _leftAndRightSpawnOffset;
				}
				_spawnRight = !_spawnRight;
			}
		}
		return gameObject;
	}
}
