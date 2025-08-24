using UnityEngine;

public class InputFieldMatchVerifier : MonoBehaviour
{
	public InputField[] inputFields;

	public GUIButton linkedButton;

	public bool forceMatch = true;

	public bool ignoreCase;

	public GameObject errorObject;

	public static bool IsOpen { get; private set; }

	private void Start()
	{
		IsOpen = true;
		if (linkedButton != null)
		{
			linkedButton.disable();
		}
		if (errorObject != null)
		{
			errorObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			LoginManager.Instance.exitMenus();
		}
	}

	private void OnDestroy()
	{
		IsOpen = false;
	}

	private bool checkForEmptyStrings()
	{
		bool flag = false;
		for (int i = 0; i < inputFields.Length; i++)
		{
			if (inputFields[i].actualString == string.Empty)
			{
				flag = true;
				break;
			}
		}
		if (flag && errorObject != null)
		{
			errorObject.SetActive(false);
			if (linkedButton != null)
			{
				linkedButton.gameObject.SetActive(true);
				linkedButton.disable();
			}
		}
		return flag;
	}

	public void OnInputFieldChanged(InputField inputField)
	{
		if (!checkForEmptyStrings())
		{
			string text = inputFields[0].actualString;
			if (ignoreCase)
			{
				text = text.ToLower();
			}
			bool flag = true;
			if (forceMatch)
			{
				for (int i = 1; i < inputFields.Length; i++)
				{
					string text2 = inputFields[i].actualString;
					if (ignoreCase)
					{
						text2 = text2.ToLower();
					}
					if (text2 != text)
					{
						flag = false;
					}
				}
			}
			if (!(linkedButton != null))
			{
				return;
			}
			if (flag)
			{
				linkedButton.gameObject.SetActive(true);
				linkedButton.enable();
				if (errorObject != null)
				{
					errorObject.SetActive(false);
				}
			}
			else
			{
				linkedButton.gameObject.SetActive(false);
				if (errorObject != null)
				{
					errorObject.SetActive(true);
				}
			}
		}
		else if (linkedButton != null)
		{
			linkedButton.gameObject.SetActive(true);
			linkedButton.disable();
		}
	}
}
