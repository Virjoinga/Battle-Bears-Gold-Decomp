using System.Collections;
using UnityEngine;

public class ScalePhysicsCollider : MonoBehaviour
{
	public float scaleFactor;

	public float maxScaleChange;

	public float scaleDelay;

	private Vector3 _lastPosition;

	private bool _canScale;

	private void Awake()
	{
		_lastPosition = base.gameObject.transform.position;
		StartCoroutine(DelayedStartScale());
	}

	private void FixedUpdate()
	{
		if (!_canScale)
		{
			return;
		}
		Vector3 position = base.gameObject.transform.position;
		if (base.gameObject.transform.localScale.x < 12f)
		{
			float num = scaleFactor * Vector3.Distance(position, _lastPosition);
			if (num > maxScaleChange)
			{
				num = maxScaleChange;
			}
			if (num > 0f)
			{
				Vector3 localScale = base.gameObject.transform.localScale;
				localScale.x += num;
				localScale.y += num;
				localScale.z += num;
				base.gameObject.transform.localScale = localScale;
			}
		}
		_lastPosition = position;
	}

	private IEnumerator DelayedStartScale()
	{
		yield return new WaitForSeconds(scaleDelay);
		_canScale = true;
	}
}
