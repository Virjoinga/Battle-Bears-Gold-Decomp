using UnityEngine;

public class MetroMouseLookBindings : MonoBehaviour
{
	public delegate bool GetCursorLocked();

	public delegate void SetCursorLocked(bool locked);

	public delegate Vector2 GetRelativeMouseMovement();

	public GetCursorLocked getCursorLocked;

	public SetCursorLocked setCursorLocked;

	public GetRelativeMouseMovement getRelativeMouseMovement;

	private static MetroMouseLookBindings _instance;

	public static MetroMouseLookBindings Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("MetroMouseLookBindings");
				_instance = gameObject.AddComponent<MetroMouseLookBindings>();
				Object.DontDestroyOnLoad(gameObject);
			}
			return _instance;
		}
	}

	public bool LockCursor
	{
		get
		{
			if (getCursorLocked != null)
			{
				return getCursorLocked();
			}
			return false;
		}
		set
		{
			if (setCursorLocked != null)
			{
				setCursorLocked(value);
			}
		}
	}

	public Vector2 RelativeMouseMovement
	{
		get
		{
			if (getRelativeMouseMovement != null)
			{
				return getRelativeMouseMovement();
			}
			return Vector2.zero;
		}
	}
}
