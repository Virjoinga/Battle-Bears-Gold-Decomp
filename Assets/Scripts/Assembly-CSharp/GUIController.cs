using System;
using System.Collections;
using UnityEngine;

public class GUIController : MonoBehaviour
{
	[SerializeField]
	private bool _isActive = true;

	public Camera guiCamera;

	public LayerMask layerMask;

	public static GameObject currentlySelectedButton;

	public bool spawnEnabled = true;

	private bool selectingButton;

	public bool IsActive
	{
		get
		{
			return _isActive;
		}
		set
		{
			_isActive = value;
			if (GUIController._activeStateChanged != null)
			{
				GUIController._activeStateChanged(_isActive, layerMask);
			}
		}
	}

	private static event Action<bool, LayerMask> _activeStateChanged;

	public static event Action<bool, LayerMask> ActiveStateChanged
	{
		add
		{
			GUIController._activeStateChanged = (Action<bool, LayerMask>)Delegate.Combine(GUIController._activeStateChanged, value);
		}
		remove
		{
			GUIController._activeStateChanged = (Action<bool, LayerMask>)Delegate.Remove(GUIController._activeStateChanged, value);
		}
	}

	private void Start()
	{
		currentlySelectedButton = null;
		if (guiCamera == null)
		{
			Transform transform = base.transform.root.Find("Camera");
			if (transform != null)
			{
				guiCamera = transform.GetComponent<Camera>();
			}
			else
			{
				guiCamera = GameObject.Find("MenuCamera").GetComponent<Camera>();
			}
		}
	}

	public void Update()
	{
		if (!_isActive)
		{
			return;
		}
		if (Input.touchCount > 0)
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				if (Input.GetTouch(i).phase != 0 && Input.GetTouch(i).phase != TouchPhase.Ended && Input.GetTouch(i).phase != TouchPhase.Moved && Input.GetTouch(i).phase != TouchPhase.Stationary)
				{
					continue;
				}
				Ray ray = guiCamera.ScreenPointToRay(Input.GetTouch(i).position);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray.origin, ray.direction, out hitInfo, 1000f, layerMask))
				{
					GameObject gameObject = hitInfo.transform.gameObject;
					if (currentlySelectedButton == null && Input.GetTouch(i).phase == TouchPhase.Began)
					{
						StopCoroutine("delayedButtonSelect");
						StartCoroutine("delayedButtonSelect", gameObject);
					}
					else if (Input.GetTouch(i).phase == TouchPhase.Ended && currentlySelectedButton != null)
					{
						if (gameObject == currentlySelectedButton)
						{
							gameObject.SendMessage("OnButtonPressed", SendMessageOptions.DontRequireReceiver);
							gameObject.SendMessage("OnButtonClicked", SendMessageOptions.DontRequireReceiver);
							currentlySelectedButton = null;
						}
						else
						{
							currentlySelectedButton.SendMessage("OnButtonDepressed", SendMessageOptions.DontRequireReceiver);
							currentlySelectedButton = null;
						}
					}
					else if (gameObject == currentlySelectedButton && !selectingButton)
					{
						gameObject.SendMessage("OnButtonPressed", SendMessageOptions.DontRequireReceiver);
					}
					else if (currentlySelectedButton != null)
					{
						currentlySelectedButton.SendMessage("OnButtonDepressed", SendMessageOptions.DontRequireReceiver);
					}
				}
				else if (currentlySelectedButton != null)
				{
					currentlySelectedButton.SendMessage("OnButtonDepressed", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		else if (currentlySelectedButton != null)
		{
			currentlySelectedButton.SendMessage("OnButtonDepressed", SendMessageOptions.DontRequireReceiver);
			currentlySelectedButton = null;
		}
	}

	private void ProcessHit(RaycastHit hit)
	{
		GameObject gameObject = hit.transform.gameObject;
		if (currentlySelectedButton == null && Input.GetMouseButtonDown(0))
		{
			StopCoroutine("delayedButtonSelect");
			StartCoroutine("delayedButtonSelect", gameObject);
		}
		else if (Input.GetMouseButtonUp(0) && currentlySelectedButton != null)
		{
			if (gameObject == currentlySelectedButton)
			{
				gameObject.SendMessage("OnButtonPressed", SendMessageOptions.DontRequireReceiver);
				gameObject.SendMessage("OnButtonClicked", SendMessageOptions.DontRequireReceiver);
				currentlySelectedButton = null;
			}
			else
			{
				currentlySelectedButton.SendMessage("OnButtonDepressed", SendMessageOptions.DontRequireReceiver);
				currentlySelectedButton = null;
			}
		}
		else if (gameObject == currentlySelectedButton && !selectingButton)
		{
			gameObject.SendMessage("OnButtonPressed", SendMessageOptions.DontRequireReceiver);
		}
		else if (currentlySelectedButton != null)
		{
			currentlySelectedButton.SendMessage("OnButtonDepressed", SendMessageOptions.DontRequireReceiver);
		}
	}

	private IEnumerator delayedButtonSelect(GameObject button)
	{
		GUIButton b = button.GetComponent(typeof(GUIButton)) as GUIButton;
		if (b != null)
		{
			selectingButton = true;
			currentlySelectedButton = button;
			yield return new WaitForSeconds(b.selectionDelay);
			if (currentlySelectedButton != null)
			{
				currentlySelectedButton.SendMessage("OnButtonPressed", SendMessageOptions.DontRequireReceiver);
			}
			selectingButton = false;
		}
	}
}
