using System;
using System.Collections;
using UnityEngine;

public class TutorialAnimator : MonoBehaviour
{
	[SerializeField]
	private GameObject[] animatingObjects;

	[SerializeField]
	private float interval = 1f;

	[SerializeField]
	private float animatingTimePerImage = 1f;

	private int currentImageIndex;

	private static TutorialAnimator instance;

	public static event Action onStartShowing;

	public static event Action onFinishShowing;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		GameObject[] array = animatingObjects;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActiveRecursively(false);
		}
	}

	private void Start()
	{
		Transform transform = null;
		if (HUD.Instance != null)
		{
			transform = HUD.Instance.hudCamera.transform;
		}
		else
		{
			Camera[] allCameras = Camera.allCameras;
			foreach (Camera camera in allCameras)
			{
				if ((camera.cullingMask & LayerMask.NameToLayer("HUD")) == LayerMask.NameToLayer("HUD"))
				{
					transform = camera.transform;
					break;
				}
			}
		}
		if (transform != null)
		{
			base.transform.position = transform.position + transform.forward * 100f;
			base.transform.forward = transform.forward;
			StartCoroutine(StartAnimating());
		}
		else if (TutorialAnimator.onFinishShowing != null)
		{
			TutorialAnimator.onFinishShowing();
		}
	}

	private IEnumerator StartAnimating()
	{
		yield return new WaitForSeconds(1f);
		if (TutorialAnimator.onStartShowing != null)
		{
			TutorialAnimator.onStartShowing();
		}
		float scaler = 0f;
		Vector3 originalScale = animatingObjects[currentImageIndex].transform.localScale;
		while (currentImageIndex < animatingObjects.GetLength(0))
		{
			animatingObjects[currentImageIndex].SetActiveRecursively(true);
			scaler += Time.deltaTime / animatingTimePerImage;
			float objectScale = 1f + Mathf.Sin(scaler * 0.75f * (float)Math.PI);
			animatingObjects[currentImageIndex].transform.localScale = originalScale * objectScale;
			yield return null;
			if (scaler >= 1f)
			{
				yield return new WaitForSeconds(interval);
				animatingObjects[currentImageIndex].SetActiveRecursively(false);
				currentImageIndex++;
				if (currentImageIndex < animatingObjects.GetLength(0))
				{
					originalScale = animatingObjects[currentImageIndex].transform.localScale;
				}
				scaler = 0f;
			}
		}
		if (TutorialAnimator.onFinishShowing != null)
		{
			TutorialAnimator.onFinishShowing();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
