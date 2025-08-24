using UnityEngine;

public class ToggleParticlesOnDisableEnable : MonoBehaviour
{
	private ParticleSystem _particleSystem;

	private void Awake()
	{
		_particleSystem = GetComponent<ParticleSystem>();
	}

	private void OnDisable()
	{
		_particleSystem.Stop();
		_particleSystem.Clear();
	}

	private void OnEnable()
	{
		_particleSystem.Play();
	}
}
