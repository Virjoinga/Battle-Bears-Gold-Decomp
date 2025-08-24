using System.Collections;
using UnityEngine;

public class GibsController : MonoBehaviour
{
	public float disappearTime = 10f;

	public float freefallTime = 1f;

	public Material skinMaterial;

	public float explosionForceAmount = 150f;

	public float upwardsForce = 100f;

	public float spinForceAmount = 250f;

	public Renderer[] renderers;

	public Rigidbody[] rigidbodies;

	public Collider[] colliders;

	private void Start()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].material = skinMaterial;
		}
		for (int j = 0; j < rigidbodies.Length; j++)
		{
			rigidbodies[j].AddForce(new Vector3(Random.Range(0f - explosionForceAmount, explosionForceAmount), upwardsForce, Random.Range(0f - explosionForceAmount, explosionForceAmount)), ForceMode.Impulse);
			rigidbodies[j].AddTorque(new Vector3(Random.Range(0f - spinForceAmount, spinForceAmount), Random.Range(0f - spinForceAmount, spinForceAmount), Random.Range(0f - spinForceAmount, spinForceAmount)), ForceMode.Impulse);
		}
		StartCoroutine(delayedDestroy());
	}

	private IEnumerator delayedDestroy()
	{
		yield return new WaitForSeconds(disappearTime);
		for (int i = 0; i < colliders.Length; i++)
		{
			Object.Destroy(colliders[i]);
		}
		yield return new WaitForSeconds(freefallTime);
		Object.Destroy(base.gameObject);
	}
}
