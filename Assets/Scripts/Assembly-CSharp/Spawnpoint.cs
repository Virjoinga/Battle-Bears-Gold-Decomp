using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
	public Team team;

	private void OnDrawGizmos()
	{
		if (team == Team.BLUE)
		{
			Gizmos.color = Color.blue;
		}
		else
		{
			Gizmos.color = Color.red;
		}
		Gizmos.DrawWireSphere(base.transform.position, 30f);
	}
}
