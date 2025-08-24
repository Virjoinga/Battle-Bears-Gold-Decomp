using UnityEngine;

public class TextArranger : MonoBehaviour
{
	public Transform[] textObjects;

	public Transform[] gastags;

	private void Awake()
	{
		Vector3 vector = base.transform.position + new Vector3(5f, 0f, 0f);
		Transform[] array = textObjects;
		foreach (Transform transform in array)
		{
			transform.position = new Vector3(vector.x, transform.position.y, transform.position.z);
		}
		vector = base.transform.position + new Vector3(-5f, 0f, 0f);
		Transform[] array2 = gastags;
		foreach (Transform transform2 in array2)
		{
			transform2.position = new Vector3(vector.x, transform2.position.y, transform2.position.z);
		}
	}

	private void OnEnable()
	{
		Vector3 vector = base.transform.position + new Vector3(5f, 0f, 0f);
		Transform[] array = textObjects;
		foreach (Transform transform in array)
		{
			transform.position = new Vector3(vector.x, transform.position.y, transform.position.z);
		}
		vector = base.transform.position + new Vector3(-5f, 0f, 0f);
		Transform[] array2 = gastags;
		foreach (Transform transform2 in array2)
		{
			transform2.position = new Vector3(vector.x, transform2.position.y, transform2.position.z);
		}
	}

	private void OnDisable()
	{
		Vector3 vector = base.transform.position + new Vector3(5f, 0f, 0f);
		Transform[] array = textObjects;
		foreach (Transform transform in array)
		{
			transform.position = new Vector3(vector.x, transform.position.y, transform.position.z);
		}
		vector = base.transform.position + new Vector3(-5f, 0f, 0f);
		Transform[] array2 = gastags;
		foreach (Transform transform2 in array2)
		{
			transform2.position = new Vector3(vector.x, transform2.position.y, transform2.position.z);
		}
	}
}
