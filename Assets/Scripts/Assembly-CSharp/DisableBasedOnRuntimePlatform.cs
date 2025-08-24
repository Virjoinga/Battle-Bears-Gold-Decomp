using UnityEngine;

public class DisableBasedOnRuntimePlatform : MonoBehaviour
{
	public bool disableInEditor;

	public bool disableOnIOS;

	public bool disableOnAndroid;

	public bool disableOnPC;

	public bool disableOnMac;

	public bool disableOnWin8;

	public bool disableMeshRendererOnly;

	private void Start()
	{
		bool flag = false;
		if ((Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) && disableInEditor)
		{
			flag = true;
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer && disableOnIOS)
		{
			flag = true;
		}
		if (Application.platform == RuntimePlatform.Android && disableOnAndroid)
		{
			flag = true;
		}
		if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) && disableOnPC)
		{
			flag = true;
		}
		if ((Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) && disableOnMac)
		{
			flag = true;
		}
		if ((Application.platform == RuntimePlatform.MetroPlayerARM || Application.platform == RuntimePlatform.MetroPlayerX64 || Application.platform == RuntimePlatform.MetroPlayerX86) && disableOnWin8)
		{
			flag = true;
		}
		if (flag)
		{
			base.gameObject.SetActive(false);
		}
	}
}
