using UnityEngine;

public class SetActiveIfMovingUp : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _objectsToSetActive;

	private float _lastYPoint;

	private void Start()
	{
		_lastYPoint = base.transform.position.y;
	}

	private void Update()
	{
		if (base.transform.position.y - _lastYPoint > 0f)
		{
			GameObject[] objectsToSetActive = _objectsToSetActive;
			foreach (GameObject gameObject in objectsToSetActive)
			{
				gameObject.SetActive(true);
			}
		}
		else
		{
			GameObject[] objectsToSetActive2 = _objectsToSetActive;
			foreach (GameObject gameObject2 in objectsToSetActive2)
			{
				gameObject2.SetActive(false);
			}
		}
		_lastYPoint = base.transform.position.y;
	}
}
