using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTracerPool : MonoBehaviour
{
	private static readonly string TRACER_ANIMATION_NAME = "tracer_anim";

	private List<Tracer> _tracerPool = new List<Tracer>();

	private int _nextTracerIndex;

	private int _tracerPoolMaxSize;

	private GameObject _tracerPrefab;

	private float _tracerPersistTime;

	public void Init(GameObject tracerPrefab, float persistTime)
	{
		_tracerPrefab = tracerPrefab;
		_tracerPersistTime = persistTime;
		switch (BBRQuality.Current)
		{
		case QualitySetting.LOWEST:
			_tracerPoolMaxSize = 2;
			break;
		case QualitySetting.LOW:
			_tracerPoolMaxSize = 4;
			break;
		case QualitySetting.MED:
			_tracerPoolMaxSize = 6;
			break;
		case QualitySetting.HIGH:
			_tracerPoolMaxSize = 8;
			break;
		case QualitySetting.ULTRA:
			_tracerPoolMaxSize = 10;
			break;
		}
		for (int i = 0; i < _tracerPoolMaxSize; i++)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(_tracerPrefab);
			gameObject.SetActive(false);
			_tracerPool.Add(new Tracer(gameObject));
		}
	}

	public void CreateTracer(Vector3 startPos, Vector3 endPos, bool hit)
	{
		Tracer tracer = _tracerPool[_nextTracerIndex];
		tracer.tracerObject.SetActive(true);
		tracer.tracerStart.position = startPos;
		tracer.tracerEnd.position = endPos;
		tracer.tracerStart.LookAt(tracer.tracerEnd);
		tracer.tracerEnd.rotation = tracer.tracerStart.rotation;
		if (hit)
		{
			tracer.tracerHead.rotation = tracer.tracerStart.rotation;
			Vector3 eulerAngles = tracer.tracerHead.eulerAngles;
			eulerAngles.z = Random.Range(0f, 360f);
			tracer.tracerHead.eulerAngles = eulerAngles;
		}
		if (tracer.tracerObject.GetComponent<Animation>() != null)
		{
			tracer.tracerObject.GetComponent<Animation>().Play(TRACER_ANIMATION_NAME);
		}
		StartCoroutine(Disable(_tracerPersistTime, tracer.tracerObject));
		_nextTracerIndex = ((_nextTracerIndex + 1 < _tracerPool.Count) ? (_nextTracerIndex + 1) : 0);
	}

	private IEnumerator Disable(float time, GameObject objectToDisable)
	{
		yield return new WaitForSeconds(time);
		objectToDisable.SetActive(false);
	}

	private void OnDisable()
	{
		foreach (Tracer item in _tracerPool)
		{
			if (item != null && item.tracerObject != null)
			{
				item.tracerObject.SetActive(false);
			}
		}
	}

	private void OnDestroy()
	{
		foreach (Tracer item in _tracerPool)
		{
			Object.Destroy(item.tracerObject);
		}
	}
}
