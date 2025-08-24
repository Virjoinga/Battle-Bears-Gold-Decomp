using UnityEngine;

public class FollowTransform : MonoBehaviour
{
	private Transform _transformToFollow;

	private GameObject _tempGameObjectToFollow;

	public void FollowTransformAtStartPoint(Transform transformToFollow, Vector3 startPoint)
	{
		if (_tempGameObjectToFollow != null)
		{
			Object.Destroy(_tempGameObjectToFollow);
		}
		_tempGameObjectToFollow = new GameObject("FollowTransformObject");
		_tempGameObjectToFollow.transform.parent = transformToFollow;
		_tempGameObjectToFollow.transform.position = startPoint;
		_transformToFollow = _tempGameObjectToFollow.transform;
	}

	private void OnDestroy()
	{
		if (_tempGameObjectToFollow != null)
		{
			Object.Destroy(_tempGameObjectToFollow);
		}
	}

	private void Update()
	{
		if (_transformToFollow != null)
		{
			base.transform.position = _transformToFollow.position;
		}
	}
}
