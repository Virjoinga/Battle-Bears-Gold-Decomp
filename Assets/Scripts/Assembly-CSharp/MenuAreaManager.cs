using UnityEngine;

public class MenuAreaManager : MonoBehaviour
{
	public MenuGroup[] menuAreas;

	public GameObject gateObject;

	public string gateOpenAnimationName;

	private bool _gateIsOpen;

	public void SetAreaActive(string areaName)
	{
		MenuGroup[] array = menuAreas;
		foreach (MenuGroup menuGroup in array)
		{
			if (areaName == menuGroup.menuAreaName)
			{
				GameObject[] menuObjects = menuGroup.menuObjects;
				foreach (GameObject gameObject in menuObjects)
				{
					gameObject.SetActive(true);
				}
				if (menuGroup.gateOpen != _gateIsOpen)
				{
					SetGateOpenState(menuGroup.gateOpen);
				}
			}
		}
	}

	public void SetOtherAreasInactive(string areaName)
	{
		MenuGroup[] array = menuAreas;
		foreach (MenuGroup menuGroup in array)
		{
			if (areaName != menuGroup.menuAreaName)
			{
				GameObject[] menuObjects = menuGroup.menuObjects;
				foreach (GameObject gameObject in menuObjects)
				{
					gameObject.SetActive(false);
				}
			}
		}
	}

	private void SetGateOpenState(bool shouldOpen)
	{
		if (_gateIsOpen && !shouldOpen)
		{
			gateObject.GetComponent<Animation>()[gateOpenAnimationName].speed = -1f;
			gateObject.GetComponent<Animation>()[gateOpenAnimationName].time = gateObject.GetComponent<Animation>()[gateOpenAnimationName].length;
			gateObject.GetComponent<Animation>().Play(gateOpenAnimationName);
			_gateIsOpen = shouldOpen;
		}
		else if (shouldOpen && !_gateIsOpen)
		{
			gateObject.GetComponent<Animation>()[gateOpenAnimationName].speed = 1f;
			gateObject.GetComponent<Animation>()[gateOpenAnimationName].time = 0f;
			gateObject.GetComponent<Animation>().Play(gateOpenAnimationName);
			_gateIsOpen = shouldOpen;
		}
	}
}
