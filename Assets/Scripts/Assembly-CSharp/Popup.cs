using System.Collections;
using UnityEngine;

public class Popup : MonoBehaviour
{
	protected GUIController disabledGUIController;

	protected GameObject callingObject;

	protected Animation myAnimation;

	protected bool enableGUIAtClose = true;

	protected GUIController ourGUIController;

	protected bool isClosing;

	protected Camera popupCamera;

	public FinishedCallback ClosingCallback;

	protected virtual void Awake()
	{
		myAnimation = base.GetComponent<Animation>();
		if (myAnimation != null && myAnimation["in"] != null)
		{
			myAnimation.Play("in");
		}
	}

	protected virtual void Start()
	{
		ourGUIController = base.transform.root.GetComponentInChildren<GUIController>();
		ourGUIController.IsActive = true;
	}

	public void OnSetCallingObject(GameObject obj, Camera cam)
	{
		callingObject = obj;
		popupCamera = cam;
		if (popupCamera != null)
		{
			popupCamera.enabled = true;
		}
	}

	public void OnSetGUIControllerToDisable(GUIController g)
	{
		disabledGUIController = g;
		disabledGUIController.IsActive = false;
	}

	protected virtual void OnClose()
	{
		base.gameObject.SetActive(true);
		if (!isClosing)
		{
			StartCoroutine(closeRoutine());
		}
	}

	private IEnumerator closeRoutine()
	{
		isClosing = true;
		if (myAnimation != null && myAnimation["out"] != null)
		{
			myAnimation.Play("out");
			yield return new WaitForSeconds(myAnimation["out"].length);
			if (base.gameObject != null)
			{
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected virtual void OnDisable()
	{
		MainMenu.numPopups--;
		if (popupCamera != null && MainMenu.numPopups <= 0)
		{
			popupCamera.enabled = false;
		}
		if (disabledGUIController != null && enableGUIAtClose && MainMenu.numPopups <= 0)
		{
			disabledGUIController.IsActive = true;
		}
		if (MainMenu.Instance != null)
		{
			MainMenu.Instance.popupClosed();
		}
	}

	protected virtual void OnEnable()
	{
		MainMenu.numPopups++;
		if (popupCamera != null)
		{
			popupCamera.enabled = true;
		}
	}
}
