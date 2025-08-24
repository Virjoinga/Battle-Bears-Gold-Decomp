using UnityEngine;

public class DestroyBasedOnRuntimePlatform : MonoBehaviour
{
	public bool destroyInEditor;

	public bool destroyOnIOS;

	public bool destroyOnAndroid;

	public bool destroyOnPC;

	public bool destroyOnMac;

	public bool destroyOnWin8;

	public bool disableMeshRendererOnly;

	private void Start()
	{
		bool flag = false;
		if ((Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) && destroyInEditor)
		{
			flag = true;
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer && destroyOnIOS)
		{
			flag = true;
		}
		if (Application.platform == RuntimePlatform.Android && destroyOnAndroid)
		{
			flag = true;
		}
		if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) && destroyOnPC)
		{
			flag = true;
		}
		if ((Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) && destroyOnMac)
		{
			flag = true;
		}
		if ((Application.platform == RuntimePlatform.MetroPlayerARM || Application.platform == RuntimePlatform.MetroPlayerX64 || Application.platform == RuntimePlatform.MetroPlayerX86) && destroyOnWin8)
		{
			flag = true;
		}
		if (disableMeshRendererOnly)
		{
			if (base.gameObject.GetComponent<Renderer>() != null)
			{
				base.gameObject.GetComponent<Renderer>().enabled = false;
			}
		}
		else if (flag)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
