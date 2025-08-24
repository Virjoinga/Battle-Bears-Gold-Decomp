using System.Collections.Generic;
using UnityEngine;

namespace SkyVuEngine.Core.Camera
{
	public class CameraManager
	{
		private List<ICamera> _cameras;

		private CameraManager _instance = null;

		[SerializeField]
		private GameObject _topDownRTSPrefab = null;

		[SerializeField]
		private GameObject _panningPrefab = null;

		public CameraManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CameraManager();
				}
				return _instance;
			}
		}

		private CameraManager()
		{
			_cameras = new List<ICamera>();
		}

		public ICamera CreateCamera(CameraType type)
		{
			ICamera camera = null;
			switch (type)
			{
			case CameraType.TopDownRTS:
				camera = Object.Instantiate(_topDownRTSPrefab) as ICamera;
				break;
			case CameraType.Panning:
				camera = Object.Instantiate(_panningPrefab) as ICamera;
				break;
			}
			if (camera == null)
			{
				return null;
			}
			_cameras.Add(camera.GetComponent<ICamera>());
			return camera;
		}

		public ICamera GetCamera(int index)
		{
			return _cameras[index];
		}

		public void DestroyCamera(int index)
		{
			ICamera camera = _cameras[index];
			_cameras.Remove(camera);
			Object.Destroy(camera.gameObject);
		}
	}
}
