using System;
using UnityEngine;

public class MogaPopUpHandler : MonoBehaviour
{
	private const string IS_MOGA_TUTORIAL_PLAYED = "IS_MOGA_TUTORIAL_PLAYED";

	private static MogaPopUpHandler instance;

	public GameObject tutorialPrefab;

	public GameObject proTutorialPrefab;

	private Action callBack;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.Log("why do i have 2 moga pop up handler");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		TutorialAnimator.onFinishShowing += OnFinishShowingTutorial;
	}

	private void OnDestroy()
	{
		TutorialAnimator.onFinishShowing -= OnFinishShowingTutorial;
	}

	private void _ShowTutorial()
	{
		if (tutorialPrefab != null && MogaController.Instance.connection == 1)
		{
			if (MogaController.Instance.GetControllerVersion() == 0)
			{
				UnityEngine.Object.Instantiate(tutorialPrefab);
			}
			else if (MogaController.Instance.GetControllerVersion() == 1)
			{
				UnityEngine.Object.Instantiate(proTutorialPrefab);
			}
		}
	}

	public static void ShowTutorial()
	{
		if (instance != null)
		{
			instance._ShowTutorial();
		}
	}

	public static void TestShowTutorial()
	{
		if (instance != null)
		{
			instance._TestShowTutorial();
		}
	}

	private void _TestShowTutorial()
	{
		if (tutorialPrefab != null)
		{
			UnityEngine.Object.Instantiate(tutorialPrefab);
		}
	}

	public static void ShowTutorialWithCallBack(Action callBack)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (callBack != null)
			{
				callBack();
			}
		}
		else if (PlayerPrefs.GetInt("IS_MOGA_TUTORIAL_PLAYED", 0) == 1)
		{
			if (callBack != null)
			{
				callBack();
			}
		}
		else if (MogaController.Instance.connection != 1)
		{
			if (callBack != null)
			{
				callBack();
			}
		}
		else
		{
			instance.callBack = callBack;
			ShowTutorial();
			PlayerPrefs.SetInt("IS_MOGA_TUTORIAL_PLAYED", 1);
		}
	}

	private void OnFinishShowingTutorial()
	{
		if (instance.callBack != null)
		{
			instance.callBack();
		}
	}
}
