using UnityEngine;

public class TargetableObject : MonoBehaviour
{
	private Team _team;

	public Team Team
	{
		get
		{
			return _team;
		}
		set
		{
			_team = value;
		}
	}
}
