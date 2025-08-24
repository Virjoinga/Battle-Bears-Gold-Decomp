using UnityEngine;

public class KOTHPoint
{
	private GameObject _satellitePrefab;

	private Vector3 _satelliteSpawnOffset = new Vector3(0f, 8000f, 0f);

	public KOTHControllerTracker tracker;

	public GameObject pointObject;

	public int pointIndex;

	private Vector3 _scaleMax;

	private float _scaleTime = 2f;

	public KOTHPoint(GameObject pointObj, int arrayIndex)
	{
		pointObject = pointObj;
		pointIndex = arrayIndex;
		tracker = pointObj.GetComponent<KOTHControllerTracker>();
		_scaleMax = pointObj.transform.localScale;
		if (KOTHManager.Instance.SatelliteResourcePrefab != null)
		{
			_satellitePrefab = (GameObject)Object.Instantiate(KOTHManager.Instance.SatelliteResourcePrefab);
			if (_satellitePrefab != null)
			{
				_satellitePrefab.SetActive(false);
			}
		}
	}

	public void PointActivated()
	{
		pointObject.SetActive(true);
		pointObject.AddComponent<AlwaysOnRadarTracker>();
		ScaleOverTimeXYZ scaleOverTimeXYZ = pointObject.AddComponent<ScaleOverTimeXYZ>();
		scaleOverTimeXYZ.StartScale = Vector3.zero;
		scaleOverTimeXYZ.EndScale = _scaleMax;
		scaleOverTimeXYZ.ScaleTime = _scaleTime;
		scaleOverTimeXYZ.SetGameObjectInActiveAfterScale = false;
		SpawnRadarTower();
	}

	public void PointDeactivated()
	{
		AlwaysOnRadarTracker component = pointObject.GetComponent<AlwaysOnRadarTracker>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		ScaleOverTimeXYZ scaleOverTimeXYZ = pointObject.AddComponent<ScaleOverTimeXYZ>();
		scaleOverTimeXYZ.StartScale = _scaleMax;
		scaleOverTimeXYZ.EndScale = Vector3.zero;
		scaleOverTimeXYZ.ScaleTime = _scaleTime;
		scaleOverTimeXYZ.SetGameObjectInActiveAfterScale = true;
		DespawnRadarTower();
	}

	private void SpawnRadarTower()
	{
		if (_satellitePrefab != null)
		{
			_satellitePrefab.SetActive(true);
			_satellitePrefab.transform.position = pointObject.transform.position + _satelliteSpawnOffset;
			MoveToPointAndCameraShake moveToPointAndCameraShake = _satellitePrefab.AddComponent<MoveToPointAndCameraShake>();
			moveToPointAndCameraShake.Destination = new Vector3(pointObject.transform.position.x, pointObject.transform.position.y - _scaleMax.y, pointObject.transform.position.z);
			moveToPointAndCameraShake.Speed = KOTHManager.Instance.SatelliteSpeed;
			moveToPointAndCameraShake.ObjectToSpawnAtPoint = KOTHManager.Instance.SatelliteExplosionPrefab;
		}
	}

	private void DespawnRadarTower()
	{
		if (_satellitePrefab != null)
		{
			MoveToPointAndDestroy moveToPointAndDestroy = _satellitePrefab.AddComponent<MoveToPointAndDestroy>();
			moveToPointAndDestroy.Acceleration = 100f;
			moveToPointAndDestroy.Destination = pointObject.transform.position + _satelliteSpawnOffset;
			moveToPointAndDestroy.MaxSpeed = KOTHManager.Instance.SatelliteSpeed;
		}
	}
}
