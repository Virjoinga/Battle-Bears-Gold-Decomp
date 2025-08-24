using System;
using System.Text.RegularExpressions;
using Analytics;
using Analytics.Parameters;
using Analytics.Schemas;
using TextFilter;
using UnityEngine;

public class PlayerNicknamePopupManager : MonoBehaviour
{
	private const string FILTER_FAIL_ERROR = "Disallowed Name";

	private const string NAME_LENGTH_ERROR = "Name Not Long Enough";

	private const int MIN_NAME_LENGTH = 3;

	private const int MAX_NAME_LENGTH = 20;

	public static string nickname = string.Empty;

	public static bool offlineMode;

	private static PlayerNicknamePopupManager _instance;

	public Action<string> NicknameSaved;

	public Action NicknameCanceled;

	public ICommunityTextFilter TextFilter;

	[SerializeField]
	private Transform _popupRoot;

	[SerializeField]
	private GameObject _nicknamePopup;

	[SerializeField]
	private AudioClip[] _clickSounds;

	private GameObject currentMenu;

	private GameObject currentStatus;

	private Camera popupCamera;

	private TextMesh _errorText;

	public static PlayerNicknamePopupManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(PlayerNicknamePopupManager)) as PlayerNicknamePopupManager;
				if (_instance == null)
				{
					return null;
				}
			}
			return _instance;
		}
	}

	public bool PopupBeingShown
	{
		get
		{
			return currentMenu != null;
		}
	}

	private void Awake()
	{
		popupCamera = _popupRoot.GetComponentInChildren<Camera>();
		if (popupCamera != null)
		{
			popupCamera.enabled = false;
		}
	}

	public void ShowNicknamePopupWithInitialName(string initialName)
	{
		nickname = initialName;
		currentMenu = UnityEngine.Object.Instantiate(_nicknamePopup) as GameObject;
		currentMenu.transform.parent = _popupRoot;
		currentMenu.transform.localPosition = new Vector3(0f, 0f, 95f);
		currentMenu.transform.localEulerAngles = Vector3.zero;
		TextMesh[] componentsInChildren = currentMenu.GetComponentsInChildren<TextMesh>();
		foreach (TextMesh textMesh in componentsInChildren)
		{
			if (textMesh.name == "errorText")
			{
				_errorText = textMesh;
				break;
			}
		}
		if (popupCamera != null)
		{
			popupCamera.enabled = true;
		}
		EventTracker.TrackEvent(new SetNicknameOpenedSchema());
	}

	public void OnInputFieldChanged(InputField inputField)
	{
		switch (inputField.name)
		{
		case "field_enter_nickname":
			if (_errorText != null)
			{
				_errorText.text = string.Empty;
			}
			nickname = inputField.actualString;
			break;
		}
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (_clickSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(_clickSounds[UnityEngine.Random.Range(0, _clickSounds.Length)], Vector3.zero);
		}
		switch (b.name)
		{
		case "closeButton":
			Cancel();
			break;
		case "saveNicknameButton":
			b.disable();
			CheckNickname();
			break;
		}
	}

	private void CheckNickname()
	{
		string text = Regex.Replace(nickname, "\\s+|\\W", string.Empty);
		if (text.Length < 3)
		{
			if (_errorText != null)
			{
				_errorText.text = "Name Not Long Enough";
			}
			return;
		}
		nickname = Regex.Replace(nickname, "\\s+", "_");
		nickname = Regex.Replace(nickname, "\\W", string.Empty);
		nickname = nickname.Substring(0, Mathf.Min(nickname.Length, 20));
		if (TextFilter != null)
		{
			TextFilter.FilterText(nickname, HandleNameFilterResult);
		}
		else
		{
			SaveAndExit();
		}
	}

	private void HandleNameFilterResult(TextFilterResult textFilter)
	{
		if (textFilter.passed)
		{
			SaveAndExit();
		}
		else if (_errorText != null)
		{
			_errorText.text = "Disallowed Name";
		}
	}

	private void SaveAndExit()
	{
		if (NicknameSaved != null)
		{
			NicknameSaved(nickname);
		}
		EventTracker.TrackEvent(new NicknameChangedSchema(new UserDisplayNameParameter(nickname)));
		ExitMenus();
	}

	private void Cancel()
	{
		if (NicknameCanceled != null)
		{
			NicknameCanceled();
		}
		ExitMenus();
	}

	private void ExitMenus()
	{
		if (currentMenu != null)
		{
			UnityEngine.Object.Destroy(currentMenu);
		}
		if (currentStatus != null)
		{
			UnityEngine.Object.Destroy(currentStatus);
		}
		if (popupCamera != null && MainMenu.numPopups <= 0)
		{
			popupCamera.enabled = false;
		}
		EventTracker.TrackEvent(new SetNicknameClosedSchema());
	}

	public void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			Cancel();
		}
	}
}
