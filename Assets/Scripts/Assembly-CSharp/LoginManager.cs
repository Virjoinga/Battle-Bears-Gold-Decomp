using System.Collections;
using Analytics;
using Analytics.Parameters;
using Analytics.Schemas;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
	public static string lastUserLoggedIn = string.Empty;

	private static LoginManager instance;

	public static bool offlineMode;

	public string currentUsername = string.Empty;

	public string currentPassword = string.Empty;

	public Transform popupRoot;

	public GameObject statusScreen;

	public GameObject noInternetMenu;

	public GameObject oldVersionMenu;

	public GameObject loginMenu;

	public GameObject createAccountMenuStep1;

	public GameObject createAccountMenuStep2;

	public GameObject forgotPasswordMenu;

	public GameObject goldVersionMenu;

	private GameObject currentMenu;

	private GameObject currentStatus;

	public AudioClip[] clickSounds;

	public AudioClip transitionSound;

	[SerializeField]
	private string _forgotPasswordUrl;

	private bool inTransition;

	private GUIController guiControllerToDisable;

	[HideInInspector]
	public string loginAttemptUsername = string.Empty;

	private string loginAttemptPassword = string.Empty;

	private string resendPasswordEmail = string.Empty;

	private string newGuestUsername = string.Empty;

	private string newGuestPassword = string.Empty;

	private Camera popupCamera;

	private string GUEST_ACCOUNT_PREFIX;

	public static LoginManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(LoginManager)) as LoginManager;
				if (instance == null)
				{
					return null;
				}
			}
			return instance;
		}
	}

	private void Awake()
	{
		GUEST_ACCOUNT_PREFIX = Language.Get("GUEST_ACCOUNT_PREFIX");
		popupCamera = popupRoot.GetComponentInChildren<Camera>();
		if (popupCamera != null)
		{
			popupCamera.enabled = false;
		}
	}

	private IEnumerator delayedOfflineMode()
	{
		yield return new WaitForSeconds(1f);
		if (popupCamera != null)
		{
			popupCamera.enabled = true;
		}
		StartCoroutine(changeMenu(noInternetMenu));
	}

	public void OnLevelWasLoaded()
	{
		if (!(Application.loadedLevelName == "Login"))
		{
			return;
		}
		if (Application.internetReachability == NetworkReachability.NotReachable || offlineMode)
		{
			offlineMode = true;
			StartCoroutine(delayedOfflineMode());
			return;
		}
		MainMenu.isFirstTime = true;
		if (PlayerPrefs.HasKey("username"))
		{
			currentUsername = PlayerPrefs.GetString("username", string.Empty);
			currentPassword = PlayerPrefs.GetString("password", string.Empty);
			if (currentUsername == string.Empty)
			{
				StartCoroutine(changeMenu(createAccountMenuStep1));
				return;
			}
			StartCoroutine(changeStatus("logging in as\n" + ((!currentUsername.StartsWith(GUEST_ACCOUNT_PREFIX)) ? currentUsername : GUEST_ACCOUNT_PREFIX)));
			ServiceManager.Instance.Login(currentUsername, currentPassword, OnLogin, OnError);
			return;
		}
		string text = OpenUDIDBinding.GetOpenUDID();
		if (text == null)
		{
			text = "super guest, since it will be common to all people";
		}
		string mD5Hash = Bootloader.Instance.GetMD5Hash(text);
		newGuestUsername = GUEST_ACCOUNT_PREFIX + mD5Hash;
		newGuestUsername += "droid";
		newGuestPassword = Bootloader.Instance.GetMD5Hash(newGuestUsername);
		ServiceManager.Instance.CreateAccount(newGuestUsername, newGuestPassword, true, OnGuestAccountCreated, OnGuestAccountCreationFailed);
	}

	public void OnInputFieldChanged(InputField inputField)
	{
		switch (inputField.name)
		{
		case "field_email":
			loginAttemptUsername = inputField.actualString;
			break;
		case "field_password":
			loginAttemptPassword = inputField.actualString;
			break;
		case "field_forgot_email":
			resendPasswordEmail = inputField.actualString;
			break;
		}
	}

	public void OnLogin()
	{
		if (currentUsername != string.Empty)
		{
			PlayerPrefs.SetString("username", currentUsername);
		}
		if (currentPassword != string.Empty)
		{
			PlayerPrefs.SetString("password", currentPassword);
		}
		lastUserLoggedIn = currentUsername;
		EventTracker.Init(ServiceManager.Instance.GetStats().pid.ToString());
		EventTracker.TrackEvent(new LoginSucceededSchema());
		ServiceManager.Instance.RefreshStoreItemList(OnGetStore, OnStoreFailed);
		PushNotifications.RegisterForPushNotifications();
	}

	public void OnLoginInGame()
	{
		if (currentUsername != string.Empty)
		{
			PlayerPrefs.SetString("username", currentUsername);
		}
		if (currentPassword != string.Empty)
		{
			PlayerPrefs.SetString("password", currentPassword);
		}
		EventTracker.TrackEvent(new LoginSucceededSchema());
		Application.LoadLevel("Login");
	}

	public void OnGetStore()
	{
		ServiceManager.Instance.RefreshPlayerLocker(OnGetLocker, OnLockerFailed);
	}

	public void OnStoreFailed()
	{
		StartCoroutine(showLoginError(ServiceManager.Instance.LastError));
	}

	public void OnGetLocker()
	{
		Store.Instance.ParseStoreInventory();
		LoadoutManager.Instance.LoadLastLoadout(ServiceManager.Instance.GetStats().pid);
		StartCoroutine(backgroundLoadMainMenu());
	}

	public void OnLockerFailed()
	{
		StartCoroutine(showLoginError(ServiceManager.Instance.LastError));
	}

	public void OnError()
	{
		string lastError = ServiceManager.Instance.LastError;
		EventTracker.TrackEvent(new LoginFailedSchema(new LoginErrorParameter(lastError)));
		if (lastError.StartsWith("VERSION: "))
		{
			StartCoroutine(changeMenu(oldVersionMenu));
		}
		else if (!lastError.StartsWith("Account disabled"))
		{
			if (!MainMenu.hasBeenToMainMenuScene)
			{
				StartCoroutine(AutoLoginFailed());
			}
			else
			{
				StartCoroutine(showLoginError(lastError));
			}
		}
	}

	private IEnumerator AutoLoginFailed()
	{
		yield return StartCoroutine(changeStatus("Automatic Login Failed\nLogging in as guest"));
		yield return new WaitForSeconds(2f);
		GuestAccountRecoveryOrCreation();
	}

	private void GuestAccountRecoveryOrCreation()
	{
		string text = OpenUDIDBinding.GetOpenUDID();
		if (text == null)
		{
			text = "super guest, since it will be common to all people";
		}
		string mD5Hash = Bootloader.Instance.GetMD5Hash(text);
		newGuestUsername = GUEST_ACCOUNT_PREFIX + mD5Hash;
		newGuestPassword = Bootloader.Instance.GetMD5Hash(newGuestUsername);
		ServiceManager.Instance.CreateAccount(newGuestUsername, newGuestPassword, true, OnGuestAccountCreated, OnGuestAccountCreationFailed);
	}

	private IEnumerator backgroundLoadMainMenu()
	{
		yield return Application.LoadLevelAsync("MainMenu");
	}

	private IEnumerator showLoginError(string error)
	{
		yield return StartCoroutine(changeStatus(error));
		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(changeMenu(loginMenu));
	}

	public IEnumerator changeMenu(GameObject newMenu)
	{
		if (currentStatus != null)
		{
			Object.Destroy(currentStatus);
		}
		if (currentMenu != null)
		{
			Animation anim = currentMenu.GetComponentInChildren<Animation>();
			if (anim["out"] != null)
			{
				inTransition = true;
				if (transitionSound != null)
				{
					AudioSource.PlayClipAtPoint(transitionSound, Vector3.zero);
				}
				anim.Play("out");
				yield return new WaitForSeconds(anim["out"].length);
			}
			Object.Destroy(currentMenu);
		}
		inTransition = false;
		if (newMenu != null)
		{
			if (popupCamera != null)
			{
				popupCamera.enabled = true;
			}
			currentMenu = Object.Instantiate(newMenu) as GameObject;
			currentMenu.transform.parent = popupRoot;
			currentMenu.transform.localPosition = Vector3.zero;
			currentMenu.transform.localEulerAngles = Vector3.zero;
		}
	}

	private IEnumerator changeStatus(string status)
	{
		yield return StartCoroutine(changeMenu(null));
		if (currentStatus == null)
		{
			if (popupCamera != null)
			{
				popupCamera.enabled = true;
			}
			currentStatus = Object.Instantiate(statusScreen) as GameObject;
			currentStatus.transform.parent = popupRoot;
			currentStatus.transform.localPosition = Vector3.zero;
			currentStatus.transform.localEulerAngles = Vector3.zero;
		}
		TextMesh statusText = currentStatus.GetComponentInChildren<TextMesh>();
		statusText.text = status;
	}

	private IEnumerator startTutorial()
	{
		if (!inTransition)
		{
			EventTracker.TrackEvent(new TutorialStartedSchema());
			Bootloader.Instance.InTutorial = true;
			yield return StartCoroutine(changeStatus("loading..."));
			yield return new WaitForSeconds(0.25f);
			yield return Application.LoadLevelAsync("Tutorial");
		}
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (clickSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(clickSounds[Random.Range(0, clickSounds.Length)], Vector3.zero);
		}
		switch (b.name)
		{
		case "closeButton":
			exitMenus();
			break;
		case "offlineTutorialButton":
			StartCoroutine(startTutorial());
			break;
		case "getNewVersionButton":
			Application.OpenURL("https://play.google.com/store/apps/details?id=net.skyvu.battlebearsgold");
			break;
		case "existingLoginButton":
			EventTracker.TrackEvent(new LoginMenuOpenedSchema());
			StartCoroutine(changeMenu(loginMenu));
			break;
		case "nextButton":
			EventTracker.TrackEvent(new AccountCreationEmailsValidatedSchema());
			StartCoroutine(changeMenu(createAccountMenuStep2));
			break;
		case "backButton":
			StartCoroutine(changeMenu(createAccountMenuStep1));
			break;
		case "createAccountButton":
			if (Application.loadedLevelName == "Login")
			{
				StartCoroutine(changeStatus("trying to create account\n" + ((!currentUsername.StartsWith(GUEST_ACCOUNT_PREFIX)) ? loginAttemptUsername : GUEST_ACCOUNT_PREFIX)));
				ServiceManager.Instance.CreateAccount(loginAttemptUsername, loginAttemptPassword, false, OnAccountCreated, OnAccountCreationFailed);
			}
			else
			{
				StartCoroutine(changeStatus("trying to upgrade account"));
				ServiceManager.Instance.UpgradeFromGuest(loginAttemptUsername, loginAttemptPassword, OnGuestAccountUpgraded, OnGuestAccountUpgradeFailed);
			}
			break;
		case "backToLoginButton":
			StartCoroutine(changeMenu(loginMenu));
			break;
		case "forgotPasswordMenuButton":
			Application.OpenURL(_forgotPasswordUrl);
			break;
		case "forgotPasswordButton":
			StartCoroutine(recoverPassword());
			break;
		case "loginButton":
			currentUsername = loginAttemptUsername;
			currentPassword = loginAttemptPassword;
			if (Application.loadedLevelName == "Login")
			{
				StartCoroutine(changeStatus("logging in as\n" + ((!currentUsername.StartsWith(GUEST_ACCOUNT_PREFIX)) ? loginAttemptUsername : GUEST_ACCOUNT_PREFIX)));
				ServiceManager.Instance.Login(loginAttemptUsername, loginAttemptPassword, OnLogin, OnError);
			}
			else
			{
				ServiceManager.Instance.Login(loginAttemptUsername, loginAttemptPassword, OnLoginInGame, OnError);
			}
			break;
		}
	}

	private void OnGuestAccountUpgraded()
	{
		EventTracker.TrackEvent(new AccountCreatedSchema(new DeviceTotalTimePlayedParameter(Bootloader.Instance.TotalTimePlayed)));
		PlayerPrefs.SetString("username", loginAttemptUsername);
		PlayerPrefs.SetString("password", loginAttemptPassword);
		Application.LoadLevel("Login");
	}

	private void OnGuestAccountUpgradeFailed()
	{
		EventTracker.TrackEvent(new AccountCreationFailedSchema(new AccountCreationErrorParameter(ServiceManager.Instance.LastError)));
		StartCoroutine(accountUpgradeFailure());
	}

	public void OnAccountCreated()
	{
		EventTracker.TrackEvent(new AccountCreatedSchema(new DeviceTotalTimePlayedParameter(Bootloader.Instance.TotalTimePlayed)));
		ServiceManager.Instance.Login(currentUsername, currentPassword, OnLogin, OnError);
	}

	public void OnAccountCreationFailed()
	{
		EventTracker.TrackEvent(new AccountCreationFailedSchema(new AccountCreationErrorParameter(ServiceManager.Instance.LastError)));
		StartCoroutine(accountCreationFailure());
	}

	public void OnGuestAccountCreated()
	{
		currentUsername = newGuestUsername;
		currentPassword = newGuestPassword;
		PlayerPrefs.SetString("username", currentUsername);
		PlayerPrefs.SetString("password", currentPassword);
		StartCoroutine(changeStatus("logging in as\n" + ((!currentUsername.StartsWith(GUEST_ACCOUNT_PREFIX)) ? currentUsername : GUEST_ACCOUNT_PREFIX)));
		ServiceManager.Instance.Login(currentUsername, currentPassword, OnLogin, OnError);
	}

	public void OnGuestAccountCreationFailed()
	{
		StartCoroutine(guestAccountCreationFailure());
	}

	private IEnumerator accountUpgradeFailure()
	{
		yield return StartCoroutine(changeStatus(ServiceManager.Instance.LastError));
		yield return new WaitForSeconds(3f);
		yield return StartCoroutine(changeMenu(createAccountMenuStep2));
	}

	private IEnumerator accountCreationFailure()
	{
		yield return StartCoroutine(changeStatus(ServiceManager.Instance.LastError));
		yield return new WaitForSeconds(3f);
		yield return StartCoroutine(changeMenu(createAccountMenuStep2));
	}

	private IEnumerator guestAccountCreationFailure()
	{
		yield return StartCoroutine(changeStatus(ServiceManager.Instance.LastError));
		yield return new WaitForSeconds(3f);
		yield return StartCoroutine(changeMenu(createAccountMenuStep1));
	}

	public void OnShowCreateAccountMenu(GUIController g)
	{
		EventTracker.TrackEvent(new AccountMenuOpenedSchema());
		guiControllerToDisable = g;
		if (guiControllerToDisable != null)
		{
			guiControllerToDisable.IsActive = false;
		}
		StartCoroutine(changeMenu(createAccountMenuStep1));
	}

	public void OnRecoverySuccess()
	{
		exitMenus();
	}

	public void OnRecoveryFailure()
	{
		StartCoroutine(passwordRecoveryFailure());
	}

	private IEnumerator recoverPassword()
	{
		StartCoroutine(changeStatus("sending email..."));
		yield return new WaitForSeconds(1f);
		ServiceManager.Instance.RequestPasswordReset(resendPasswordEmail, OnRecoverySuccess, OnRecoveryFailure);
	}

	private IEnumerator passwordRecoveryFailure()
	{
		yield return StartCoroutine(changeStatus(ServiceManager.Instance.LastError));
		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(changeMenu(forgotPasswordMenu));
	}

	public void exitMenus()
	{
		if (Application.loadedLevelName == "Login")
		{
			StartCoroutine(changeMenu(createAccountMenuStep1));
			return;
		}
		EventTracker.TrackEvent(new AccountMenuClosedSchema());
		if (currentMenu != null)
		{
			Object.Destroy(currentMenu);
		}
		if (currentStatus != null)
		{
			Object.Destroy(currentStatus);
		}
		if (popupCamera != null)
		{
			popupCamera.enabled = false;
		}
		if (guiControllerToDisable != null)
		{
			guiControllerToDisable.IsActive = true;
		}
	}
}
