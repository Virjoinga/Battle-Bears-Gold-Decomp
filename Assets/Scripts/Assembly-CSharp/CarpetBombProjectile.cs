using System.Collections;
using UnityEngine;

public class CarpetBombProjectile : Projectile
{
	[SerializeField]
	private float _carpetBombDuration = 4f;

	[SerializeField]
	private float _bombInterval = 1f;

	[SerializeField]
	private float _undergroundSpeed = 100f;

	private bool _hasHitGround;

	private new void Start()
	{
		Item itemByName = ServiceManager.Instance.GetItemByName(spawnItemOverride);
		itemByName.UpdateProperty("duration", ref _carpetBombDuration, base.EquipmentNames);
		itemByName.UpdateProperty("explosion_interval", ref _bombInterval, base.EquipmentNames);
		itemByName.UpdateProperty("undergroundProjectileSpeed", ref _undergroundSpeed, base.EquipmentNames);
	}

	private void Update()
	{
		if (!_hasHitGround)
		{
			base.transform.forward = base.rigidbody.velocity.normalized;
		}
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		_hasHitGround = true;
		CalculateFacingDirection(collision);
		base.rigidbody.isKinematic = true;
		DisableMeshRenderers();
		StartCoroutine(DoCarpetBomb());
	}

	private void CalculateFacingDirection(Collision collision)
	{
		Vector3 velocity = base.rigidbody.velocity;
		base.rigidbody.useGravity = false;
		base.transform.position -= collision.contacts[0].normal * base.collider.bounds.extents.x;
		Vector3 realNormal = GetRealNormal(collision.contacts[0].point, collision.contacts[0].normal, collision.contacts[0].otherCollider);
		SetForwardFromNormal(velocity, realNormal);
	}

	private void SetForwardFromNormal(Vector3 velocity, Vector3 contactNormal)
	{
		Vector3 vector = Vector3.Project(velocity, contactNormal);
		velocity -= vector;
		if (velocity != Vector3.zero)
		{
			base.transform.forward = velocity.normalized;
		}
		else
		{
			base.transform.forward = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z);
		}
	}

	private Vector3 GetRealNormal(Vector3 contactPoint, Vector3 contactNormal, Collider otherCollider)
	{
		float distance = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		RaycastHit[] array = Physics.RaycastAll(position, contactPoint - position, distance, 1 << otherCollider.gameObject.layer);
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (raycastHit.collider == otherCollider)
			{
				return raycastHit.normal;
			}
		}
		return contactNormal;
	}

	private void DisableMeshRenderers()
	{
		SkinnedMeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		SkinnedMeshRenderer[] array = componentsInChildren;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			skinnedMeshRenderer.enabled = false;
		}
		ParticleSystem[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array2 = componentsInChildren2;
		foreach (ParticleSystem particleSystem in array2)
		{
			particleSystem.Stop();
		}
	}

	private IEnumerator DoCarpetBomb()
	{
		float startTime = Time.fixedTime;
		float nextExplosionTime = Time.fixedTime;
		while (Time.fixedTime - startTime < _carpetBombDuration)
		{
			GetFacingDirectionFromRaycast();
			base.transform.position += base.transform.forward * _undergroundSpeed * Time.deltaTime;
			if (Time.fixedTime >= nextExplosionTime)
			{
				nextExplosionTime = Time.fixedTime + _bombInterval;
				CreateExplosion();
				DoCameraShake();
			}
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	private void GetFacingDirectionFromRaycast()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(base.transform.position, Vector3.down), out hitInfo))
		{
			SetForwardFromNormal(base.transform.forward * _undergroundSpeed, hitInfo.normal);
		}
	}
}
