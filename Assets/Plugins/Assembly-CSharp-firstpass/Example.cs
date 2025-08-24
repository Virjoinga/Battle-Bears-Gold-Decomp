using UnityEngine;

public class Example : MonoBehaviour
{
	private Controller mController;

	private bool mFocused;

	private readonly Vector3 mMaxScale = new Vector3(8f, 8f, 8f);

	private readonly Vector3 mMinScale = new Vector3(0.1f, 0.1f, 0.1f);

	private readonly Vector3 mStepScale = new Vector3(0.1f, 0.1f, 0.1f);

	private GameObject mPlayer;

	private void Awake()
	{
		mPlayer = GameObject.Find("Cube");
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject instance = Controller.getInstance(@static);
		mController = new Controller(instance);
		mController.init();
		if (mFocused)
		{
			mController.onResume();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		mFocused = focus;
		if (mController != null)
		{
			if (mFocused)
			{
				mController.onResume();
			}
			else
			{
				mController.onPause();
			}
		}
	}

	private void OnDestroy()
	{
		mController.exit();
		mController = null;
	}

	private void Update()
	{
		int state = mController.getState(1);
		int keyCode = mController.getKeyCode(108);
		int keyCode2 = mController.getKeyCode(96);
		int keyCode3 = mController.getKeyCode(97);
		float axisValue = mController.getAxisValue(0);
		float axisValue2 = mController.getAxisValue(1);
		float axisValue3 = mController.getAxisValue(11);
		float axisValue4 = mController.getAxisValue(14);
		if (keyCode == 0)
		{
			mPlayer.transform.position = Vector3.zero;
			mPlayer.transform.localScale = Vector3.one;
		}
		else
		{
			if (keyCode2 == 0)
			{
				mPlayer.transform.localScale -= mStepScale;
				mPlayer.transform.localScale = Vector3.Max(mPlayer.transform.localScale, mMinScale);
			}
			if (keyCode3 == 0)
			{
				mPlayer.transform.localScale += mStepScale;
				mPlayer.transform.localScale = Vector3.Min(mPlayer.transform.localScale, mMaxScale);
			}
		}
		mPlayer.transform.position += new Vector3(axisValue + axisValue3, 0f - (axisValue2 + axisValue4), 0f) * 0.5f;
		mPlayer.transform.localEulerAngles += Vector3.up;
		if (state == 1)
		{
			mPlayer.GetComponent<Renderer>().material.color = Color.green;
		}
		else
		{
			mPlayer.GetComponent<Renderer>().material.color = Color.red;
		}
	}
}
