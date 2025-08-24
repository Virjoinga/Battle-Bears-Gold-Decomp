using System.Collections.Generic;
using UnityEngine;

public class RaycastEffectDiscController : DiscController
{
	private static readonly float _epsilon = 0.5f;

	private static readonly float _defaultShotLength = 4000f;

	[SerializeField]
	private LayerMask _raycastMask;

	[SerializeField]
	private GameObject _tracerPrefab;

	private List<GameObject> _activeTracers = new List<GameObject>();

	private int _lastTracerUsedIndex;

	public override void Start()
	{
		base.Start();
		_activeTracers.Add(Object.Instantiate(_tracerPrefab) as GameObject);
		_activeTracers.Add(Object.Instantiate(_tracerPrefab) as GameObject);
	}

	public override void Update()
	{
		base.Update();
		DoShootEffect();
		DoShootEffect();
	}

	private void DoShootEffect()
	{
		if (_spawnPoints.Length > 0)
		{
			int spawnIndex = GetSpawnIndex();
			Vector3 position = _spawnPoints[spawnIndex].position;
			Vector3 vector = new Vector3(Random.value * 2f - 1f, -0.5f, Random.value * 2f - 1f);
			Vector3 end = position + vector * _defaultShotLength;
			RaycastHit hitInfo;
			if (Physics.Raycast(position, vector, out hitInfo, _defaultShotLength, _raycastMask))
			{
				end = hitInfo.point;
			}
			SpawnTracer(position, end, _spawnPoints[spawnIndex]);
		}
	}

	private void SpawnTracer(Vector3 start, Vector3 end, Transform spawnPoint)
	{
		if (_tracerPrefab != null)
		{
			if (_lastTracerUsedIndex >= _activeTracers.Count)
			{
				_lastTracerUsedIndex = 0;
			}
			GameObject gameObject = _activeTracers[_lastTracerUsedIndex++];
			Transform transform = gameObject.transform.Find("start");
			Transform transform2 = gameObject.transform.Find("end");
			Transform transform3 = transform2.Find("Tracer_Head");
			gameObject.transform.parent = spawnPoint;
			gameObject.transform.position = start;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			transform.position = start;
			transform2.position = end;
			if (transform3 != null)
			{
				transform3.LookAt(transform2.position + end - start);
			}
		}
	}

	protected override void Expired()
	{
		base.Expired();
		for (int i = 0; i < _activeTracers.Count; i++)
		{
			if (_activeTracers[i] != null)
			{
				Object.Destroy(_activeTracers[i]);
			}
		}
	}
}
