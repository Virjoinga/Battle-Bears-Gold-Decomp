using UnityEngine;

public abstract class NetworkObject : MonoBehaviour
{
	private int ownerID;

	private float damageMultiplier = 1f;

	private float meleeMultiplier = 1f;

	public int OwnerID
	{
		get
		{
			return ownerID;
		}
		set
		{
			ownerID = value;
		}
	}

	public float DamageMultiplier
	{
		get
		{
			return damageMultiplier;
		}
		set
		{
			damageMultiplier = value;
		}
	}

	public float MeleeMultiplier
	{
		get
		{
			return meleeMultiplier;
		}
		set
		{
			meleeMultiplier = value;
		}
	}
}
