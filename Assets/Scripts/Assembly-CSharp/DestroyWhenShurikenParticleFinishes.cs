using UnityEngine;

public class DestroyWhenShurikenParticleFinishes : MonoBehaviour
{
	public ParticleSystem linkedParticles;

	private void LateUpdate()
	{
		if (!linkedParticles.IsAlive(true))
		{
			Object.Destroy(base.gameObject);
		}
	}
}
