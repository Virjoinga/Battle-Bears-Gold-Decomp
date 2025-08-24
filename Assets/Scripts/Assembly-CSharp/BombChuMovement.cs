using UnityEngine;

public class BombChuMovement : MonoBehaviour
{
	private Vector3 _raycastDownOffset = new Vector3(0f, 20f, 0f);

	private float _groundSpeed = 300f;

	private bool _hasHitGround;

	[SerializeField]
	private LayerMask _raycastMask;

	private void Update()
	{
		if (_hasHitGround)
		{
			GetFacingDirectionAndPositionFromRaycast();
		}
		base.transform.forward = base.rigidbody.velocity;
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		base.rigidbody.useGravity = false;
		base.rigidbody.detectCollisions = false;
		_hasHitGround = true;
		base.rigidbody.velocity = base.transform.rotation * new Vector3(0f, 0f, _groundSpeed);
		Physics.IgnoreCollision(base.collider, collision.collider);
		CalculateFacingDirection(collision);
	}

	private void CalculateFacingDirection(Collision collision)
	{
		Vector3 velocity = base.rigidbody.velocity;
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

	private void GetFacingDirectionAndPositionFromRaycast()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(base.transform.position + _raycastDownOffset, Vector3.down), out hitInfo, 50f, _raycastMask))
		{
			base.transform.position = hitInfo.point;
			SetForwardFromNormal(base.transform.forward * _groundSpeed, hitInfo.normal);
		}
		else
		{
			base.rigidbody.useGravity = true;
			base.rigidbody.detectCollisions = true;
			_hasHitGround = false;
		}
	}
}
