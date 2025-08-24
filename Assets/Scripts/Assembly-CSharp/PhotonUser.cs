using ExitGames.Client.Photon;
using UnityEngine;

public abstract class PhotonUser : MonoBehaviour
{
	private int userID = -1;

	public bool isLocal;

	public Hashtable UserParameters { get; private set; }

	public int UserID
	{
		get
		{
			return userID;
		}
	}

	public abstract void OnCreate(bool isLocal);

	public abstract void OnDestroy();

	private void Start()
	{
		base.enabled = false;
	}

	public void Construct(int userId, Hashtable userParams, bool local)
	{
		UserParameters = userParams;
		userID = userID;
		isLocal = local;
	}

	public void OnCreateObject()
	{
		OnCreate(isLocal);
	}

	public void Destroy()
	{
		userID = -1;
		UserParameters = null;
		OnDestroy();
		Object.DestroyImmediate(this);
	}
}
