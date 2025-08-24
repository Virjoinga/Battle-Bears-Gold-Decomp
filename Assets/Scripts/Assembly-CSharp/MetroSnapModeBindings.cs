using UnityEngine;

public class MetroSnapModeBindings : MonoBehaviour
{
	private static MetroSnapModeBindings _instance;

	private bool _isCurrentlySnapped;

	public static MetroSnapModeBindings Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("MetroSnapModeBindings");
				_instance = gameObject.AddComponent<MetroSnapModeBindings>();
				Object.DontDestroyOnLoad(gameObject);
			}
			return _instance;
		}
	}

	public void ApplicationSnapped()
	{
		_isCurrentlySnapped = true;
		Tutorial tutorial = Object.FindObjectOfType(typeof(Tutorial)) as Tutorial;
		if (tutorial != null)
		{
			tutorial.QuittingEarly();
		}
		if (!LoginManager.offlineMode)
		{
			if (Application.loadedLevelName != "Tutorial" && PhotonManager.Instance != null)
			{
				PhotonManager.Instance.Leave();
				ServiceManager.Instance.LogGameLeft("user_quit");
			}
			Application.LoadLevel("MetroSnapped");
		}
		else
		{
			Application.LoadLevel("MetroSnapped");
		}
	}

	public void ApplicationUnsnapped()
	{
		if (_isCurrentlySnapped)
		{
			_isCurrentlySnapped = false;
			if (LoginManager.offlineMode)
			{
				Application.LoadLevel("Login");
			}
			else
			{
				Application.LoadLevel("MainMenu");
			}
		}
	}
}
