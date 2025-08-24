using UnityEngine;

public class MainCameraManager : MonoBehaviour
{
	[SerializeField]
	private Camera _mainCamera;

	[SerializeField]
	private Camera _popupCamera;

	[SerializeField]
	private Transform _popupsRoot;

	private int _popupCount;

	private void Awake()
	{
		_popupCount = _popupsRoot.childCount;
	}

	private void Update()
	{
		if (_popupsRoot.childCount > _popupCount)
		{
			_popupCount = _popupsRoot.childCount;
			SwitchMainCamera(_popupCamera, _mainCamera);
		}
		else if (_popupsRoot.childCount < _popupCount)
		{
			_popupCount = _popupsRoot.childCount;
			SwitchMainCamera(_mainCamera, _popupCamera);
		}
	}

	private void SwitchMainCamera(Camera newMainCamera, Camera oldMainCamera)
	{
		newMainCamera.tag = "MainCamera";
		oldMainCamera.tag = "Untagged";
	}
}
