using System.Collections;
using UnityEngine;

public class GUIButton : MonoBehaviour
{
	public AnimatedScroller scroller;

	public bool inactive;

	public GameObject listener;

	public float selectionDelay;

	public GameObject pressObj;

	public GameObject upObj;

	public GameObject greyObj;

	private Transform myTransform;

	public bool disabled;

	public bool unpressWhenReleased;

	public float unpressDelay = 1f;

	private void Awake()
	{
		myTransform = base.transform;
		if (pressObj == null)
		{
			foreach (Transform item in myTransform)
			{
				if (item.name.StartsWith("press"))
				{
					pressObj = item.gameObject;
				}
			}
		}
		if (upObj == null)
		{
			foreach (Transform item2 in myTransform)
			{
				if (item2.name.StartsWith("up"))
				{
					upObj = item2.gameObject;
				}
			}
		}
		if (greyObj == null)
		{
			foreach (Transform item3 in myTransform)
			{
				if (item3.name.StartsWith("grey"))
				{
					greyObj = item3.gameObject;
				}
			}
		}
		if (disabled)
		{
			disable();
		}
		else
		{
			enable();
		}
		if (pressObj != null)
		{
			pressObj.SetActive(false);
		}
	}

	public bool isDisabled()
	{
		return disabled;
	}

	public void greyify()
	{
		if (greyObj != null)
		{
			if (pressObj != null)
			{
				pressObj.GetComponent<Renderer>().material = greyObj.GetComponent<Renderer>().material;
			}
			if (upObj != null)
			{
				upObj.GetComponent<Renderer>().material = greyObj.GetComponent<Renderer>().material;
			}
		}
	}

	public void disable()
	{
		disabled = true;
		if (base.GetComponent<Collider>() != null)
		{
			base.GetComponent<Collider>().enabled = false;
		}
		if (greyObj != null)
		{
			greyObj.SetActive(true);
		}
		if (upObj != null)
		{
			upObj.SetActive(false);
		}
		if (pressObj != null)
		{
			pressObj.SetActive(false);
		}
	}

	public void showDisableButAllowClicks()
	{
		if (base.GetComponent<Collider>() != null)
		{
			base.GetComponent<Collider>().enabled = true;
		}
		if (greyObj != null)
		{
			greyObj.SetActive(true);
		}
		if (upObj != null)
		{
			upObj.SetActive(false);
		}
		if (pressObj != null)
		{
			pressObj.SetActive(false);
		}
	}

	public void enable()
	{
		disabled = false;
		if (base.GetComponent<Collider>() != null)
		{
			base.GetComponent<Collider>().enabled = true;
		}
		if (greyObj != null)
		{
			greyObj.SetActive(false);
		}
		if (upObj != null)
		{
			upObj.SetActive(true);
		}
		if (pressObj != null)
		{
			pressObj.SetActive(false);
		}
	}

	private void OnEnable()
	{
		inactive = false;
		if (disabled)
		{
			disable();
		}
		else
		{
			enable();
		}
	}

	private void OnDisable()
	{
		inactive = true;
		if (greyObj != null && greyObj.transform != null)
		{
			greyObj.SetActive(false);
		}
		if (upObj != null && upObj.transform != null)
		{
			upObj.SetActive(false);
		}
		if (pressObj != null && pressObj.transform != null)
		{
			pressObj.SetActive(false);
		}
	}

	private void Update()
	{
		if (scroller != null && scroller.didScroll)
		{
			if (upObj != null)
			{
				upObj.SetActive(true);
			}
			if (pressObj != null)
			{
				pressObj.SetActive(false);
			}
		}
	}

	public void OnButtonPressed()
	{
		if (!disabled && !inactive && (!(scroller != null) || !scroller.didScroll))
		{
			if (pressObj != null)
			{
				pressObj.SetActive(true);
			}
			if (upObj != null)
			{
				upObj.SetActive(false);
			}
			SendMessageUpwards("OnGUIButtonPressed", this, SendMessageOptions.DontRequireReceiver);
			if (listener != null)
			{
				listener.SendMessage("OnGUIButtonPressed", this, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void OnButtonDepressed()
	{
		if (!disabled && !inactive && (!(scroller != null) || !scroller.didScroll))
		{
			if (upObj != null)
			{
				upObj.SetActive(true);
			}
			if (pressObj != null)
			{
				pressObj.SetActive(false);
			}
		}
	}

	public void OnButtonClicked()
	{
		if (disabled || inactive)
		{
			return;
		}
		if (unpressWhenReleased)
		{
			StartCoroutine(delayedUnpress());
		}
		if (!(scroller != null) || !scroller.didScroll)
		{
			SendMessageUpwards("OnGUIButtonClicked", this, SendMessageOptions.DontRequireReceiver);
			if (listener != null)
			{
				listener.SendMessage("OnGUIButtonClicked", this, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private IEnumerator delayedUnpress()
	{
		yield return new WaitForSeconds(unpressDelay);
		OnButtonDepressed();
	}
}
