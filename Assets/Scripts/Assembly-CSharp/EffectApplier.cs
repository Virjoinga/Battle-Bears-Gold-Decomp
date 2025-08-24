using UnityEngine;

public abstract class EffectApplier : MonoBehaviour
{
	[SerializeField]
	protected string itemOverride;

	protected virtual void OnTriggerEnter(Collider collider)
	{
		PlayerController component = collider.gameObject.GetComponent<PlayerController>();
		if (component != null)
		{
			ApplyEffect(component);
		}
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		PlayerController component = base.GetComponent<Collider>().gameObject.GetComponent<PlayerController>();
		if (component != null)
		{
			ApplyEffect(component);
		}
	}

	protected abstract void ApplyEffect(PlayerController pc);
}
